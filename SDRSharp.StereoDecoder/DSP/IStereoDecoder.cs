using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDRSharp.StereoDecoder
{
    unsafe interface IStereoDecoder
    {
        bool ForceMono
        {
            get; 
            set; 
        }

        bool IsPllLocked
        {
            get;
        }
        float DeemphasisTime
        {
            get;
            set;
        }
        void Process(float* baseBand, float* interleavedStereo, int length);
        void Configure(double sampleRate, int DecimationRatio);
    }
}
