using SDRSharp.Radio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDRSharp.StereoDecoder
{
    class AudioDumper
    {
        //Audio goes here
        private UnsafeBuffer _AudioBuffer;
        private unsafe float* _AudioPtr;

        private SimpleWavWriter _wavWriter;
        int SampleRate = 0;
        public void Configure(int sampleRate)
        {
            this.SampleRate = sampleRate;

            var dateTime = DateTime.Now;
            var dateString = dateTime.ToString("yyyyMMdd");
            var timeString = dateTime.ToString("HHmmssZ");

            string FileName = string.Format("debug_{0}_{1}.wav", dateString, timeString);

            _wavWriter = new SimpleWavWriter(FileName, WavSampleFormat.Float32, (uint)sampleRate);
            _wavWriter.Open();
        }

        public unsafe void Dump(double SampleRate, float* data, int length, bool ScaleAudio = false)
        {
            if (_wavWriter == null)
                Configure((int)SampleRate);
            else
            {
                #region Initialize audio buffer
                if (_AudioBuffer == null || _AudioBuffer.Length != length)
                {
                    _AudioBuffer = UnsafeBuffer.Create(length, sizeof(float));
                    _AudioPtr = (float*)_AudioBuffer;
                }
                #endregion

                //If we need to scale audio
                //Utils.Memcpy(_AudioPtr, data, length * sizeof(float));
                if (ScaleAudio)
                {
                    for (var i = 0; i < length; i++)
                    {
                        _AudioPtr[i] = data[i];
                        _AudioPtr[i] *= 1000000;
                    }
                }
                _wavWriter.Write(_AudioPtr, length);
            }
        }
        public unsafe void Dump(float* data, int length)
        {
            _wavWriter.Write(data, length);
        }
    }
}
