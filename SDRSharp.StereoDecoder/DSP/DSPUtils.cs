using SDRSharp.Radio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using SDRSharp.StereoDecoder.Algebra;

namespace SDRSharp.StereoDecoder
{
    public class DSPUtils
    {
        public static Complex[] RealToComplex(float[] in_buffer)
        {
            Complex[] out_buffer = new Complex[in_buffer.Length];
            for (int i = 0; i < out_buffer.Length; i++)
            {
                out_buffer[i].Real = in_buffer[i];
                out_buffer[i].Imag = 0f;
            }
            return out_buffer;
        }
        public unsafe static void RealToComplex(float* in_buffer, Complex* out_buffer, int length)
        {
            for (int i = 0; i < length; i++)
            {
                out_buffer[i].Real = in_buffer[i];
                out_buffer[i].Imag = 0f;
            }
        }
        public unsafe static float[] ComplexToReal(Complex[] in_buffer)
        {
            float[] out_buffer = new float[in_buffer.Length];
            for (int i = 0; i < out_buffer.Length; i++)
            {
                out_buffer[i] = in_buffer[i].Real;
            }
            return out_buffer;
        }
        public unsafe static void ComplexToReal(Complex* in_buffer, float* out_buffer, int length)
        {
            for (int i = 0; i < length; i++)
            {
                out_buffer[i] = in_buffer[i].Real;
            }
        }
        public unsafe static void ComplexToRealFast(Complex* in_buffer, float* out_buffer, int length)
        {
            Utils.Memcpy(out_buffer, in_buffer, length * 4);
        }


        //This is matlab filter function, implemented in C#
        public static unsafe void filter(float[] b, float[] a, float* input, float* output, int length)
        {

            //Checks if these conditions are met otherwise it
            //will return the original input x
            if (a[0] != 0f && (a.Length >= b.Length))
            {

                int n = b.Length;

                //Filter delay filled with zeros
                float[] z = new float[n];
                fillZeros2(z);

                //The filtered signal filled with zeros
                float[] Y = new float[length];
                fillZeros2(Y);

                //Divide b and a by first coefficient of a
                divideEach2(ref b, a[0]);
                divideEach2(ref a, a[0]);

                for (int m = 0; m < Y.Length; m++)
                {

                    //Calculates the filtered value using
                    Y[m] = b[0] * input[m] + z[0];


                    for (int i = 1; i < n; i++)
                    {

                        //Previous filter delays recalculated by
                        z[i - 1] = b[i] * input[m] + z[i] - a[i] * Y[m];

                    }

                }

                //Trims the last element off of filter delay
                float[] zC = (float[])z.Clone();
                z = new float[zC.Length - 1];
                for (int i = 0; i < z.Length; i++)
                    z[i] = zC[i];

                //Copy filtered signal to output buffer
                for (int i = 0; i < length; i++)
                {
                    output[i] = Y[i];
                }
                //Utils.Memcpy(output, Y, length * sizeof(float));
                return;
            }

            //Returns original signal when conditions not met
            //Copy filtered signal to output buffer
            for (int i = 0; i < length; i++)
            {
                output[i] = input[i];
            }
            return;
        }

        //Divides a float array by a decimal
        private static void divideEach2(ref float[] array, float divisor)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i] / divisor;
            }
        }
        private static Complex[] divideEach(Complex[] array, float divisor)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i] / divisor;
            }
            return array;
        }

        //Multiplies a float array by a decimal
        private static Complex[] multiplyEach(Complex[] array, float scalar)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i] * scalar;
            }
            return array;
        }

        //Adds a float array by a decimal
        private static Complex[] addEach(Complex[] array, float scalar)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i] + scalar;
            }
            return array;
        }

        //Substracts a float array by a decimal
        private static Complex[] substractEach(Complex[] array, float scalar)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i] + scalar;
            }
            return array;
        }

        private static Complex prod(Complex[] vector)
        {
            Complex product = vector[0]; 
            for (int i = 0; i < vector.Length; i++)
                product *= vector[i];
            return product;
        }

        //This method will divide each element of one vector, by each element of another vector
        //Example [4 5 6]./[1 2 3] -> 4/1=4, 5/2=2.5, 6/3=2 -> [4.0 2.5 2.0]
        private static Complex[] divEachVec(Complex[] vector, Complex[] vector2)
        {
            Complex[] product = new Complex[vector.Length];
            for (int i = 0; i < vector.Length; i++)
                product[i] = vector[i] / vector2[i];
            return product;
        }


        //Fills a big decimal array with zeros
        private static void fillZeros2(float[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 0f;
            }
        }

        private static void fillZeros(Complex[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 0f;
            }
        }

        //Fills a big decimal array with ones
        private static void fillOnes(Complex[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 1f;
            }
        }

        //Fills a big decimal array with negative ones
        private static void fillNegativeOnes(Complex[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = -1f;
            }
        }
        /// <summary>
        /// Append the scalar value c to the vector x until it is of length l.
        /// </summary>
        /// <param name="x">Vector</param>
        /// <param name="l">Length you want your new vector to be</param>
        /// <param name="c">Value you want your new vector to be padded with</param>
        /// <returns></returns>
        public static Complex[] postpad(Complex[] x, int l, float c)
        {
            Complex[] result = new Complex[l];

            //Fill new vector with previous vector
            for (int i = 0; i < x.Length; i++)
                result[i] = x[i];

            //Fill the remainder of new vector with value til length
            for (int i = x.Length; i < result.Length; i++)
                result[i] = c;

            return result; 
        }


        /// <summary>
        /// Convert transfer function filter parameters to zero-pole-gain form
        /// Find the zeros, poles, and gains of this continuous-time system
        /// </summary>
        /// <param name="b">numerator</param>
        /// <param name="a">denominator</param>
        /// <returns>z (zeros), p (poles), g (gain)</returns>
        public static ArrayList tf2zp(Complex[] b, Complex[] a)
        {
            if (b.Length != a.Length)
                throw new ArgumentException("b and a must have the same length.");

            float g = b[0].Real / a[0].Real;
            Complex[] z = LinearAlgebra.Roots(b);
            Complex[] p = LinearAlgebra.Roots(a);

            return new ArrayList { z, p, g };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="z">zeros</param>
        /// <param name="p">poles</param>
        /// <param name="k">gain</param>
        /// <returns>polynomial transfer function representation from zeros and poles</returns>
        public static ArrayList zp2tf(Complex[] z, Complex[] p, float k)
        {
            return zpk2tf(z, p, k);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="z">zeros</param>
        /// <param name="p">poles</param>
        /// <param name="k">gain</param>
        /// <returns>polynomial transfer function representation from zeros and poles</returns>
        public static ArrayList zpk2tf(Complex[] z, Complex[] p, float k)
        {
            Complex[] zCoefficients = LinearAlgebra.FromRoots(z);
            Complex[] bCoefficients = new Complex[zCoefficients.Length];

            for (int i = 0; i < zCoefficients.Length; i++)
                bCoefficients[i] = zCoefficients[i] * k;

            Complex[] aCoefficients = LinearAlgebra.FromRoots(p);
            Array.Reverse(aCoefficients);

            return new ArrayList { aCoefficients, bCoefficients };
        }

        public static ArrayList bilinear(Complex[] Sz, Complex[] Sp, float Sg) //Sg=T=fs(sample rate)
        {
            
            float T = Sg;
            var arlist1 = DSPUtils.tf2zp(Sz, Sp);

            Sz = (Complex[])arlist1[0];
            Sp = (Complex[])arlist1[1];
            Sg = (float)arlist1[2];

            return zpkBilinear(Sz, Sz, (int)Sg, T);
        }
        public static ArrayList bilinear(Complex[] Sz, Complex[] Sp, float Sg, float T) //T=fs(sample rate)
        {
            return zpkBilinear(Sz, Sp, (int)Sg, T);
        }
        public static ArrayList zpkBilinear(Complex[] z, Complex[] p, int k, double fs)
        {
            //Return a digital filter from an analog one using a bilinear transform.
            Complex[] zz = new Complex[z.Length];
            Complex[] pz = new Complex[p.Length];
            double kz;
            relativeDegree(z, p);
            double fs2 = 2 * fs;
            Complex complexFs2 = new Complex((float)fs2, 0f);
            for (int i = 0; i < z.Length; i++)
            {
                zz[i] = (complexFs2 + z[i]) / (complexFs2 - z[i]);
            }
            for (int i = 0; i < p.Length; i++)
            {
                pz[i] = (complexFs2 + p[i]) / (complexFs2 - p[i]);
            }

            //rearranging pz
            List<Complex> pz2 = new List<Complex>();
            foreach (Complex aPz in pz)
            {
                if (!pz2.Contains(aPz) && !pz2.Contains(aPz.Conjugate()))
                {
                    pz2.Add(aPz);
                }
            }

            for (int i = 0; i < pz.Length / 2; i++)
                pz2.Add(pz2[i].Conjugate());

            Complex[] pzRearranged = pz2.ToArray();
            
            Complex temp = new Complex(1, 0);
            foreach (Complex aZ in z) temp = temp * (complexFs2 - aZ);

            Complex temp2 = new Complex(1, 0);
            foreach (Complex aP in p) temp2 = temp2 * (complexFs2 - aP);
            
            kz = (k * (temp / temp2).Real);

            var arlist2 = DSPUtils.zp2tf(zz, pzRearranged, (float)kz);

            Complex[] Zz = (Complex[])arlist2[0];
            Complex[] Zp = (Complex[])arlist2[1];

            return new ArrayList { Zz, Zp };
        }

        //In an LTI system, the relative degree is the difference between the degree of the transfer function's denominator polynomial (i.e., number of poles) 
        //and the degree of its numerator polynomial (i.e., number of zeros).
        private int relativeDegree(double[] z, Complex[] p)
        {
            int degree = p.Length - z.Length;
            if (degree < 0)
            {
                throw new ArgumentException("Improper transfer function.");
            }
            else
                return degree;
        }

        private static int relativeDegree(Complex[] z, Complex[] p)
        {
            int degree = p.Length - z.Length;
            if (degree < 0)
            {
                throw new ArgumentException("Improper transfer function.");
            }
            else
                return degree;
        }
    }
}
