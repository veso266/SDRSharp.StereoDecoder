using SDRSharp.Radio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDRSharp.StereoDecoder
{
    public unsafe class StereoDecoder : IStereoDecoder
    {
        IStereoDecoder st = new CCIRStereoDecoder();

        private float _deemphasisTime = (float)Utils.GetDoubleSetting("deemphasisTime", 50) * 1e-6f;

        private double _sampleRate;
        private bool _forceMono;

        private string _stereoSystem = string.Empty; //Default stereo system is CCIR
        int _decimationRatio;
        public bool ForceMono
        {
            get { return _forceMono; }
            set { _forceMono = value; }
        }
        public string StereoSystem
        {
            get
            {
                return _stereoSystem;
            }
            set
            {
                this._stereoSystem = value;
            }
        }

        public bool IsPllLocked
        {
            get { return this.st.IsPllLocked; }
        }

        public float DeemphasisTime
        {
            get { return _deemphasisTime / 1e-6f; }
            set { _deemphasisTime = value * 1e-6f; }
        }
        public void Process(float* baseBand, float* interleavedStereo, int length)
        {
            st.Process(baseBand, interleavedStereo, length);
        }

        public void Configure(double sampleRate, int decimationRatio)
        {
            if (_sampleRate != sampleRate)
            {
                _sampleRate = sampleRate;
                _decimationRatio = decimationRatio;
                st.Configure(_sampleRate, decimationRatio);
            }
        }

        public void reConfigure(float deemphasisTime, bool forceMono)
        {
            this._deemphasisTime = deemphasisTime;
            this._forceMono = forceMono;

            //Configure what Stereo System are we using
            //We should take care so that our classes will not be inicialized all the time, because if they are, then we get pops in the audio
            if (_stereoSystem == "CCIR")
            {
                var type = st.GetType();
                if (type != typeof(CCIRStereoDecoder))
                {
                    st = new CCIRStereoDecoder();
                    st.Configure(_sampleRate, _decimationRatio);
                }
            }
            else if (_stereoSystem == "Polar")
            {
                var type = st.GetType();
                if (type != typeof(OIRTStereoDecoder))
                {
                    st = new OIRTStereoDecoder();
                    st.Configure(_sampleRate, _decimationRatio);
                }
            }
            else if (_stereoSystem == "Auto")
            {
                //YOU HAVE TO REMEMBER!!!: 
                /* 
                 * This function will not continuously detect stereo system (It will detect it and then set it for you)
                 * If for some reason the system shall change it will not detect that
                 * I know I could calculate DTFT for that, but why would I bring extra overhead to the computer for something that usualy does not happen without human noticing
                 * I can not implement this with swiching between CCIR and Polar, because even if its done very fast, you will hear that a pop in the audio
                 */
                _stereoSystem = DetectStereo();
            }

            st.DeemphasisTime = deemphasisTime;
            st.ForceMono = forceMono;
        }

        private string DetectStereo()
        {
            //How would we detect what stereo system are we using? 
            //What we need to do is to see where out Pilot tone is
            //If its on 19khz we have CCIR
            //If its on 31.25kht we have Polar Stereo
            //We have 3 methods, that I know of for detecting frequencies in the signal

            //1. Autocorelation: 
            /*
             *  With this method we colerate the signal by a shifted version of itself 
             *  and we can extraxt fundamental frequencies (strongest ones), 
             *  the problem is that pilot tone is not the strongest frequency in the MPX signal, not even CCIR pilot let alone Polar one (which is realy weak and gets easly overpowered)
             *  
             *  So we can not use this method to detect the stereo system      
             *      
             *  If you would like to detect fundamental frequencies in audio signal you can see an example here: https://dsp.stackexchange.com/a/15499
             */

            //2. FFT (Fast Fourier transform):
            /*
             * This one looks promising, it will compute DFT of our signal in time domain (it can so do space domain) 
             * and will get us all the frequencies in it (even the weaker ones), so then we can just see if there is anythig at 19 or 31.25khz
             * 
             * But although, this one will do the job, its still requires some CPU cycles and we can do simpler
             * 
             * If you want to read more about it, check out wikipedia: https://en.wikipedia.org/wiki/Fast_Fourier_transform#Cooley%E2%80%93Tukey_algorithm
             * Or if you would like to see an example of how to use it take a look at Noisy signal example here: https://www.mathworks.com/help/matlab/ref/fft.html
             */

            //3. Try and fail
            /*
             * Simple and efective, we will try to lock to CCIR pilot first (at 19khz)
             *  Why CCIR first? Because if you look at an MPX CCIR stereo signal: https://c8fde164-a-62cb3a1a-s-sites.googlegroups.com/site/kygriffinky/home/510/fm-stereo-multiplex/1-31%20fm%20sterreo.jpeg
             *  You will see that from 16 to 22khz there is nothing but the pilot, nothing to bother the detector, so if it exist it will always lock, it is not overpowered by L-R diference like Polar pilot is
             *  
             *  If it locks, we have CCIR Stereo
             *  If it does not lock, we try Pilot at 31.25khz and if it locks there, we have Polar stereo
             *  
             *  If it locks neather on CCIR or Polar one, then we probably have a mono signal :)
             */

            if (st.IsPllLocked && st.GetType() == typeof(CCIRStereoDecoder))
                return "CCIR";
            else if (!st.IsPllLocked && st.GetType() == typeof(CCIRStereoDecoder)) //We try polar
            {
                st = new OIRTStereoDecoder();
                st.Configure(_sampleRate, _decimationRatio);
                if (st.IsPllLocked && st.GetType() == typeof(OIRTStereoDecoder))
                    return "Polar";
            }

            return "CCIR";
        }
    }
}
