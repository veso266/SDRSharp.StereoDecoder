using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SDRSharp.Radio;

namespace SDRSharp.StereoDecoder.Algebra
{
    /// <summary>
    /// a class that defines a polynomial with complex coefficients
    /// modified frm http://sites.google.com/site/drjohnbmatthews/polyrots
    /// </summary>
    class ComplexPolynomial
    {
        /// <summary>
        /// The array of coefficients with the highest power at coefficientss[0]
        /// </summary>
        private Complex[] a;

        /// <summary>
        /// create a polynomial with real and complex coefficients
        /// </summary>
        /// <param name="coefficients">the array of coefficients with the highest power at coefficientss[0]</param>
        public ComplexPolynomial(Complex[] coefficients)
        {
            a = new Complex[coefficients.Length];
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = new Complex(coefficients[i]);
            }
        }

        /// <summary>
        /// create a polynomial with real coefficients
        /// </summary>
        /// <param name="coefficients">the array of coefficients with the highest power at coefficientss[0]</param>
        public ComplexPolynomial(params float[] coefficients)
        {
            a = new Complex[coefficients.Length];
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = new Complex(coefficients[i], 0);
            }
        }

        /// <summary>
        /// Evaluate the polynomial at x using Horner's scheme.
        /// </summary>
        /// <param name="x">the Complex value of the function at x.</param>
        /// <returns></returns>
        public Complex evaluate(Complex x)
        {
            Complex result = a[0];
            for (int i = 1; i < a.Length; i++)
            {
                result = (result * x) + a[i];
            }
            return result;
        }

        /// <summary>
        /// This implementation uses the Durand-Kerner-Weierstrass method
        /// to find the rots of a polynomial with complex coefficients.
        /// The method requires a monic polynomial; some errr may occur
        /// when dividing by the leading coefficient.
        /// The array should have the highest order coefficient first.
        /// </summary>
        /// <param name="max">the maximum iteration</param>
        /// <param name="errr">the errr that is acceptable</param>
        /// <returns>an array of the Complex roots of the polynomial.</returns>
        public Complex[] Roots(int max, double errr)
        {
            Complex one = new Complex(1, 0);
            if (a[0] != one)
            {
                //copy the coefficients so that it will not be changed
                Complex[] ca = new Complex[a.Length];
                for (int i = 0; i < a.Length; i++)
                {
                    ca[i] = new Complex(a[i]);
                }

                for (int i = 1; i < ca.Length; i++)
                {
                    ca[i] = ca[i] / ca[0];
                }
                ca[0].Real = 1;
                ca[0].Imag = 0;
                ComplexPolynomial poly = new ComplexPolynomial(ca);
                return poly.Roots(max, errr);
            }

            //the polynomial is monix
            //i.e coefficient of x^n is 1

            Complex[] r = new Complex[a.Length - 1];            //the array of rots
                                                                //Initialize r
                                                                //the n-1 rots are
                                                                //(0.4, 0.9)^0
                                                                //(0.4, 0.9)^1
                                                                //(0.4, 0.9)^2
                                                                //(0.4, 0.9)^3
                                                                //(0.4, 0.9)^4
                                                                //etc
                                                                //no particular reason to choose (0.4, 0.9)
            Complex multiplier = new Complex(0.4f, 0.9f);
            r[0] = new Complex(1, 0);
            for (int i = 1; i < r.Length; i++)
            {
                r[i] = r[i - 1] * multiplier;
            }
            //Iteration
            int count = 0;
            bool done = false;
            while (!done)
            {
                bool unchanged = true;
                for (int i = 0; i < r.Length; i++)
                {
                    Complex numerator = evaluate(r[i]);

                    Complex denominator = new Complex(1, 0);
                    for (int j = 0; j < r.Length; j++)
                    {
                        if (i != j)
                        {
                            denominator = (r[i] - r[j]) * denominator;
                        }
                    }

                    Complex newRi = r[i] - (numerator / denominator);
                    float realDiff = r[i].Real - newRi.Imag;
                    float imgDiff = r[i].Imag - newRi.Imag;
                    if (realDiff < 0)
                    {
                        realDiff = -realDiff;
                    }
                    if (imgDiff < 0)
                    {
                        imgDiff = -imgDiff;
                    }
                    if ((realDiff > errr) || (imgDiff > errr))
                    {
                        unchanged = false;
                    }
                    //replace r[i] with new value
                    r[i] = new Complex(newRi);
                }
                count++;
                done = (count > max || unchanged);
                if (count > max)
                {
                    //throw new Exception("Exceeded");
                }
            }
            return r;
        }


    }
    public static class LinearAlgebra
    {        
        /// <summary>
        /// Calculates the complex roots of the Polynomial by eigenvalue decomposition
        /// </summary>
        /// <param name="Coefficients">The coefficients of the polynomial in a</param>
        /// <returns>a vector of complex numbers with the roots</returns>
        public static Complex[] Roots(Complex[] Coefficients)
        {
            ComplexPolynomial poly = new ComplexPolynomial(Coefficients);
            Complex[] roots = poly.Roots(999, 1E-15);
            Array.Reverse(roots);
            return roots;
        }


        /// <summary>
        /// vieta's formulas to calculate polynomial coefficients from roots
        /// </summary>
        /// <param name="roots"></param>
        /// <returns>Returns a Complex array of coefficients of the polynomial</returns>
        public static Complex[] FromRoots(Complex[] roots)
        {
            int n = roots.Length;
            Complex[] coeffs = new Complex[n + 1];
            coeffs[n] = 1.0f;

            for (int i = 0; i < n; i++)
            {
                for (int j = n - i - 1; j < n; j++)
                {
                    coeffs[j] = coeffs[j] - roots[i] * coeffs[j + 1];
                }
            }

            Array.Reverse(coeffs);
            return coeffs;
        }
    }
}
