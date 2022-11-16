using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SDRSharp.Radio;

namespace SDRSharp.StereoDecoder
{
    public unsafe class CCIRStereoDecoder : IStereoDecoder
    {
        private const int DefaultPilotFrequency = 19000;
        private const int PllRange = 20;
        private const int PllBandwith = 10;
        private const float PllThreshold = 1.0f;
        private const float PllLockTime = 0.5f; // sec
        private const float PllZeta = 0.707f;
        private const float AudioGain = 0.2f;

        private static readonly float _pllPhaseAdjM = (float)Utils.GetDoubleSetting("pllPhaseAdjM", 0.0f);
        private static readonly float _pllPhaseAdjB = (float)Utils.GetDoubleSetting("pllPhaseAdjB", -1.75);
        private static readonly bool _isMultiThreaded = Environment.ProcessorCount > 1;

        private float _deemphasisTime = (float)Utils.GetDoubleSetting("deemphasisTime", 50) * 1e-6f;
        public double PilotFrequency
        {
            get
            {
                return _pll->DefaultFrequency;
            }
            set
            {
                _pll->DefaultFrequency = (float)value;
            }
        }
        public float DeemphasisTime
        {
            get { return _deemphasisTime / 1e-6f; }
            set { _deemphasisTime = value * 1e-6f; }
        }

        private readonly SharpEvent _event = new SharpEvent(false);
        private readonly Pll* _pll;
        private readonly UnsafeBuffer _pllBuffer;

        private IirFilter* _pilotFilter;
        private UnsafeBuffer _pilotFilterBuffer;
        private UnsafeBuffer _channelABuffer;
        private UnsafeBuffer _channelBBuffer;
        private float* _channelAPtr;
        private float* _channelBPtr;
        private FirFilter _channelAFilter;
        private FirFilter _channelBFilter;
        private FloatDecimator _channelADecimator;
        private FloatDecimator _channelBDecimator;
        private double _sampleRate;
        private int _audioDecimationFactor;
        private float _deemphasisAlpha;
        private float _deemphasisAvgL;
        private float _deemphasisAvgR;
        private bool _forceMono;

        public CCIRStereoDecoder()
        {
            _pllBuffer = UnsafeBuffer.Create(sizeof(Pll));
            _pll = (Pll*)_pllBuffer;
        }

        public bool ForceMono
        {
            get { return _forceMono; }
            set { _forceMono = value; }
        }

        public bool IsPllLocked
        {
            get { return _pll->IsLocked; }
        }
        public void Process(float* baseBand, float* interleavedStereo, int length)
        {
            if (_forceMono)
            {
                ProcessMono(baseBand, interleavedStereo, length);
            }
            else
            {
                ProcessStereo(baseBand, interleavedStereo, length);
            }
        }

        private void ProcessMono(float* baseBand, float* interleavedStereo, int length)
        {
            #region Prepare buffer

            if (_channelABuffer == null || _channelABuffer.Length != length)
            {
                _channelABuffer = UnsafeBuffer.Create(length, sizeof(float));
                _channelAPtr = (float*)_channelABuffer;
            }

            #endregion

            #region Decimate L+R

            Utils.Memcpy(_channelAPtr, baseBand, length * sizeof(float));
            _channelADecimator.Process(_channelAPtr, length);

            #endregion

            #region Filter L+R

            length /= _audioDecimationFactor;
            _channelAFilter.Process(_channelAPtr, length);

            #endregion

            #region Process deemphasis

            if (_deemphasisTime != 0) //If user wants De-Emphasis
            {
                for (var i = 0; i < length; i++)
                {
                    _deemphasisAvgL += _deemphasisAlpha * (_channelAPtr[i] - _deemphasisAvgL);
                    _channelAPtr[i] = _deemphasisAvgL;
                }
            }

            #endregion

            #region Fill output buffer

            for (var i = 0; i < length; i++)
            {
                var sample = _channelAPtr[i] * AudioGain;
                interleavedStereo[i * 2] = sample;
                interleavedStereo[i * 2 + 1] = sample;
            }

            #endregion
        }

        private void ProcessStereo(float* baseBand, float* interleavedStereo, int length)
        {
            #region Prepare L+R buffer

            if (_channelABuffer == null || _channelABuffer.Length != length)
            {
                _channelABuffer = UnsafeBuffer.Create(length, sizeof(float));
                _channelAPtr = (float*)_channelABuffer;
            }

            #endregion

            #region Prepare L-R buffer

            if (_channelBBuffer == null || _channelBBuffer.Length != length)
            {
                _channelBBuffer = UnsafeBuffer.Create(length, sizeof(float));
                _channelBPtr = (float*)_channelBBuffer;
            }

            #endregion

            #region Decimate and filter L+R

            var audioLength = length / _audioDecimationFactor;

            if (_isMultiThreaded)
            {
                DSPThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        Utils.Memcpy(_channelAPtr, baseBand, length * sizeof(float));
                        _channelADecimator.Process(_channelAPtr, length);
                        _channelAFilter.Process(_channelAPtr, audioLength);
                        _event.Set();
                    });
            }
            else
            {
                Utils.Memcpy(_channelAPtr, baseBand, length * sizeof(float));
                _channelADecimator.Process(_channelAPtr, length);
                _channelAFilter.Process(_channelAPtr, audioLength);
            }

            #endregion

            #region Demodulate L-R

            for (var i = 0; i < length; i++)
            {
                var pilot = _pilotFilter->Process(baseBand[i]);
                _pll->Process(pilot);
                _channelBPtr[i] = baseBand[i] * Trig.Sin((float)(_pll->AdjustedPhase * 2.0));
            }

            if (!_pll->IsLocked)
            {
                if (_isMultiThreaded)
                {
                    _event.WaitOne();
                }

                #region Process mono deemphasis

                for (var i = 0; i < audioLength; i++)
                {
                    _deemphasisAvgL += _deemphasisAlpha * (_channelAPtr[i] - _deemphasisAvgL);
                    _channelAPtr[i] = _deemphasisAvgL;
                }

                #endregion

                #region Fill output buffer with mono

                for (var i = 0; i < audioLength; i++)
                {
                    var sample = _channelAPtr[i] * AudioGain;
                    interleavedStereo[i * 2] = sample;
                    interleavedStereo[i * 2 + 1] = sample;
                }

                #endregion

                return;
            }

            #endregion

            #region Decimate and filter L-R

            _channelBDecimator.Process(_channelBPtr, length);
            _channelBFilter.Process(_channelBPtr, audioLength);

            #endregion

            #region Recover L and R audio channels

            if (_isMultiThreaded)
            {
                _event.WaitOne();
            }

            for (var i = 0; i < audioLength; i++)
            {
                var a = _channelAPtr[i];
                var b = 2f * _channelBPtr[i];
                interleavedStereo[i * 2] = (a + b) * AudioGain;
                interleavedStereo[i * 2 + 1] = (a - b) * AudioGain;
            }

            #endregion

            #region Process deemphasis

            if (_deemphasisTime != 0) //If user wants De-Emphasis
            {
                for (var i = 0; i < audioLength; i++)
                {
                    _deemphasisAvgL += _deemphasisAlpha * (interleavedStereo[i * 2] - _deemphasisAvgL);
                    interleavedStereo[i * 2] = _deemphasisAvgL;

                    _deemphasisAvgR += _deemphasisAlpha * (interleavedStereo[i * 2 + 1] - _deemphasisAvgR);
                    interleavedStereo[i * 2 + 1] = _deemphasisAvgR;
                }
            }

            #endregion
        }

        public void Configure(double sampleRate, int DecimationRatio)
        {
            _audioDecimationFactor = (int)Math.Pow(2.0, DecimationRatio); //4 = 2^2

            if (_sampleRate != sampleRate)
            {
                _sampleRate = sampleRate;

                _pilotFilterBuffer = UnsafeBuffer.Create(sizeof(IirFilter));
                _pilotFilter = (IirFilter*)_pilotFilterBuffer;
                _pilotFilter->Init(IirFilterType.BandPass, DefaultPilotFrequency, _sampleRate, 500);


                _pll->SampleRate = (float)_sampleRate;
                _pll->DefaultFrequency = DefaultPilotFrequency;
                _pll->Range = PllRange;
                _pll->Bandwidth = PllBandwith;
                _pll->Zeta = PllZeta;
                _pll->PhaseAdjM = _pllPhaseAdjM;
                _pll->PhaseAdjB = _pllPhaseAdjB;
                _pll->LockTime = PllLockTime;
                _pll->LockThreshold = PllThreshold;

                var outputSampleRate = sampleRate / _audioDecimationFactor;  //48000 = 192000/4
                var coefficients = FilterBuilder.MakeBandPassKernel(outputSampleRate, 250, Vfo.MinBCAudioFrequency, Vfo.MaxBCAudioFrequency, WindowType.BlackmanHarris4);
                _channelAFilter = new FirFilter(coefficients, 1);
                _channelBFilter = new FirFilter(coefficients, 1);

                _deemphasisAlpha = (float)(1.0 - Math.Exp(-1.0 / (outputSampleRate * _deemphasisTime)));
                _deemphasisAvgL = 0;
                _deemphasisAvgR = 0;
            }

            if (_channelADecimator == null || _channelBDecimator == null || _audioDecimationFactor != _channelADecimator.DecimationRatio)
            {
                _channelADecimator = new FloatDecimator(_audioDecimationFactor);
                _channelBDecimator = new FloatDecimator(_audioDecimationFactor);
            }
        }
    }
}