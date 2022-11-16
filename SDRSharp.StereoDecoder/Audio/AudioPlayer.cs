using System;
using System.Diagnostics;
using SDRSharp.Common;
using SDRSharp.Radio;
using SDRSharp.Radio.PortAudio;

namespace SDRSharp.StereoDecoder
{
    public class AudioPlayer
    {
        public int DeviceIndex
        {
            set
            {
                this._deviceIndex = value;
            }
        }

        public float Gain
        {
            set
            {
                this._gain = value;
            }
        }

        public int LostBuffers
        {
            get
            {
                return this._lostBuffers;
            }
        }

        public int BufferSize
        {
            get
            {
                return this._bufferSize;
            }
        }

        public int InternalSampleRate
        {
            get
            {
                return (int)(this._sampleRate) / 1000;
            }
        }

        public bool ForceMono
        {
            get { return this._forceMono; }
            set { this._forceMono = value; }
        }
        public float DeemphasisTime
        {
            get
            {
                return _deemphasisTime;
            }
            set
            {
                this._deemphasisTime = value;
            }
        }

        public string StereoSystem
        {
            get
            {
                return this.st.StereoSystem;
            }
            set
            {
                this.st.StereoSystem = value;
            }
        }

        public bool IsPllLocked
        {
            get { return _PilotLocked; }
        }

        private const float OutputLatency = 0.1f;

        private FloatFifoStream _audioStream;
        private AudioProcessor _audioProcessor;
        private WavePlayer _wavePlayer;
        private Resampler _resampler;
        private UnsafeBuffer _InputBuffer;
        private unsafe float* _InputBufferPtr;
        private int _deviceIndex;
        private double _sampleRateOut;
        private int _outputLength;
        private float _gain;
        private int _lostBuffers;
        private int _bufferSize;
        private int _maxBufferSize;
        private double _sampleRate;
        private int _inputLength;

        private bool _forceMono;
        private float _deemphasisTime;
        private bool _PilotLocked;
        
        //Audio out
        private UnsafeBuffer audioBuffer;
        private unsafe float* audioBufferPtr;

        //Stereo
        StereoDecoder st = new StereoDecoder();

        //SDRSharpControl
        ISharpControl control;

        public unsafe AudioPlayer(ISharpControl control, AudioProcessor audioProcessor)
        {
            this.control = control;
            this._audioProcessor = audioProcessor;
            this._audioProcessor.AudioReady += this.AudioSamplesIn;
            this._audioProcessor.Enabled = false;
        }

        public void Start()
        {
            this._lostBuffers = 0;
            this._audioStream = new FloatFifoStream(BlockMode.None);
            this._audioProcessor.Enabled = true;
        }

        public unsafe void Stop()
        {
            this._audioProcessor.Enabled = false;
            if (this._wavePlayer != null)
            {
                this._wavePlayer.Dispose();
                this._wavePlayer = null;
            }
            if (this._audioStream != null)
            {
                this._audioStream.Close();
                this._audioStream = null;
            }
            if (this._resampler != null)
            {
                this._resampler = null;
                this.audioBuffer.Dispose();
                this.audioBuffer = null;
                this.audioBufferPtr = null;
                this._InputBuffer.Dispose();
                this._InputBuffer = null;
                this._InputBufferPtr = null;
            }
            this._sampleRate = 0.0;
            this._sampleRateOut = 0.0;
            this._bufferSize = 0;
        }

        /// <summary>
        /// This method is called whenever SDR# gives samples to it
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="samplerate"></param>
        /// <param name="length"></param>
        private unsafe void AudioSamplesIn(float* buffer, double samplerate, int length)
        {
            if (this._wavePlayer == null || samplerate != this._sampleRate)
            {
                this._sampleRate = samplerate;         //Every device is different, it can be 192khz, 300khz, 384khz, etc
                //this._sampleRateOut = 96000;
                this._sampleRateOut = this.control.AudioSampleRate; //Normaly 48000
                this._maxBufferSize = (int)this._sampleRate;

                //We multiplay our Length by 0.1 so audio device doesn't run out of samples (hear silence when that happens)
                this._inputLength = (int)(this._sampleRate * OutputLatency);
                this._outputLength = (int)(this._sampleRateOut * OutputLatency);

                #region Initialize buffers
                //Input buffer
                if (this._InputBuffer == null || this._InputBuffer.Length != length)
                {
                    this._InputBuffer = UnsafeBuffer.Create(this._inputLength, sizeof(float));
                    this._InputBufferPtr = (float*)this._InputBuffer;
                }
                //Audio
                if (this.audioBuffer == null || this.audioBuffer.Length != length)
                {
                    //Buffer needs to be twice as large because we are in stereo
                    this.audioBuffer = UnsafeBuffer.Create(this._inputLength, sizeof(float));
                    this.audioBufferPtr = (float*)audioBuffer;
                }
                #endregion

                #region Init Audio player
                if (this._wavePlayer != null)
                {
                    this._wavePlayer.Dispose();
                    this._wavePlayer = null;
                }
                this._wavePlayer = new WavePlayer(this._deviceIndex, this._sampleRateOut, this._outputLength, new AudioBufferNeededDelegate(this.PlayerProcess));
            }
            if (this._audioStream.Length >= this._maxBufferSize)
            {
                this._lostBuffers++;
                return;
            }
            #endregion

            #region Configure Stereo Decoder

            /// <summary>
            /// Audio decimation factor or Decimation for short
            /// is a value that tells the computer how many samples should it take when resampling down
            /// If we are resampling from 192khz to 48khz, we take 192000/48000 = 4 samples
            /// </summary>
            int DecimationRatio = (int)Math.Round(Math.Sqrt((int)this._sampleRate / (int)this._sampleRateOut)); //-> (192000 / 48000) = sqrt(4) = 2
            if (this._sampleRate <= this._sampleRateOut)
                DecimationRatio = 0;
            st.Configure(this._sampleRate, DecimationRatio);

            #endregion

            this._audioStream.Write(buffer, length); //Send samples to a intermediate FIFO buffer, where PlayerProcess will later take them from 
        }

        /// <summary>
        /// This method is called whenever soundcard wants samples (you should feed them to it, else, you hear nothing)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        private unsafe void PlayerProcess(float* buffer, int length)
        {
            if (this._audioStream == null)
            {
                return;
            }
            this._bufferSize = (int)Math.Min((float)this._audioStream.Length / (float)this._maxBufferSize * 100f, 100f);
            if (this._audioStream.Length < this._inputLength)
            {
                this._lostBuffers++;
                for (int i = 0; i < length; i++)
                {
                    buffer[i] = 0f;
                }
                return;
            }

            //Read the audiostream (samples) into InputBufferPtr (InputBufferPtr holds our audio direcly from SDR#)
            this._audioStream.Read(this._InputBufferPtr, this._inputLength);

            //Reconfigure Stereo Decoder
            st.reConfigure(this._deemphasisTime, this._forceMono);

            //Process Stereo
            st.Process(this._InputBufferPtr, buffer, this._inputLength);

            //Scale Audio output
            this.BoostStereoOutput(buffer, this._outputLength);

            //Pilot tone light
            this._PilotLocked = st.IsPllLocked;
        }

        private unsafe void BoostStereoOutput(float* buffer, int length)
        {
            for (var i = 0; i < length; i++)
            {
                var sampleL = buffer[i * 2 + 1] * this._gain;
                var sampleR = buffer[i * 2] * this._gain;
                buffer[i * 2 + 1] = sampleL;                     //Left Channel
                buffer[i * 2] = sampleR;                        //Right Channel
            }
        }


        /* I could also scale the audio like this, but the upper method is more understandable
        private unsafe void BoostStereoOutput(float* buffer, int length)
        {
            for (var i = 0; i < length; i++)
            {
                buffer[i] *= this._gain;
            }
        }*/
    }
}