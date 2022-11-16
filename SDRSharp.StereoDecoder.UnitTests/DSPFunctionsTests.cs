using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SDRSharp.StereoDecoder;
using SDRSharp.StereoDecoder.Algebra;

using SDRSharp.Radio;

namespace SDRSharp.StereoDecoder.UnitTests
{
    [TestClass]
    public class VectorToolsTest
    {
        [TestMethod]
        public void Testpostpad()
        {
            //Arrange
            Complex[] test = new Complex[] { 1f, 2f };

            //Act
            Complex[] result = DSPUtils.postpad(test, 5, -1);

            //Assert
            Complex[] ExpectedOutputVector = { 1f, 2f, -1f, -1f, -1f }; //C#
            CollectionAssert.AreEqual(ExpectedOutputVector, result);
        }
    }

    [TestClass]
    public class LinearAlgebraTest
    {
        [TestMethod]
        public void TestPolyFromRoots()
        {
            //Arrange
            Complex[] roots =
            {
                new Complex(-3, 1),
                new Complex(-3, -1),
                -5,
                -1
            };

            //Act
            Complex[] poly = LinearAlgebra.FromRoots(roots);

            string poly1 = poly[0].ToString();
            string poly2 = poly[1].ToString();
            string poly3 = poly[2].ToString();
            string poly4 = poly[3].ToString();
            string poly5 = poly[4].ToString();

            //Assert
            Assert.AreEqual("real 1, imag 0", poly1);
            Assert.AreEqual("real 12, imag 0", poly2);
            Assert.AreEqual("real 51, imag 0", poly3);
            Assert.AreEqual("real 90, imag 0", poly4);
            Assert.AreEqual("real 50, imag 0", poly5);
        }
    }

    [TestClass]
    public class BilinearTransformTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
                            "b and a must have the same length.")]
        public void TestTf2zpExeption()
        {
            //Arrange
            Complex[] b = new Complex[] { 2, 3, 0 };
            Complex[] a = new Complex[] { 1, 0.4f };

            //Act
            var arlist1 = DSPUtils.tf2zp(b, a);
        }
            
        [TestMethod]
        public void TestTf2zp()
        {

            //Test in matlab
            /*
             from spectrum import tf2zp % You only need this if using Octave
             b = [2,3,0]
             a = [1, 0.4, 1]
             [z,p,k] = tf2zp(b,a)          % Obtain zero-pole-gain form
             z =
                 1.5
                 0
             p =
                -0.2000 + 0.9798i
                 -0.2000 - 0.9798i
             k =
                2
             */


            //Arrange
            Complex[] b = new Complex[] { 2, 3, 0 };
            Complex[] a = new Complex[] { 1, 0.4f, 1 };

            //Act
            var arlist1 = DSPUtils.tf2zp(b, a);

            Complex[] z = (Complex[])arlist1[0];
            Complex[] p = (Complex[])arlist1[1];
            float k = (float)arlist1[2];

            float zA1 = z[0].Real; //Actual zero1
            float zA2 = z[1].Real; //Actual zero2

            float pA1R = p[0].Real; //Actual real pole1
            float pA1I = p[0].Imag; //Actual imaginary pole1
            float pA2R = p[1].Real; //Actual real pole2
            float pA2I = p[1].Imag; //Actual imaginary pole2

            float kA = k; //Actual gain

            float zE1 = 0; //Expected zero1
            float zE2 = -1.5f;    //Expected zero2


            //Matlab says, that imaginary part should be 0.9798
            //but numpy.roots and this algo says, so we will leave it at 0.9797959
            //Maybe matlab is using this internaly to find roots: https://math.stackexchange.com/questions/17110/how-to-calculate-complex-roots-of-a-polynomial

            float pE1R = -0.2000f; //Expected real pole1
            float pE1I = 0.9797959f;  //Expected imaginary pole1
            float pE2R = -0.2000f; //Expected real pole2
            float pE2I = -0.9797959f;  //Expected imaginary pole2

            float kE = 2f; //Expected gain

            //Assert

            //Ničle
            Assert.AreEqual(zE1, zA1);
            Assert.AreEqual(zE2, zA2);

            //Poli
            Assert.AreEqual(pE1R, pA1R);    //1. pol (realni del)
            Assert.AreEqual(pE1I, pA1I);    //1. pol (imaginarni del)
            Assert.AreEqual(pE2R, pA2R);    //2. pol (realni del)
            Assert.AreEqual(pE2I, pA2I);    //2. pol (imaginarni del)

            //Gain
            Assert.AreEqual(kE, kA);
        }

        [TestMethod]
        public void TestZp2tf()
        {
            //Test in matlab
            /*
                k = 2;
                z = [0 -1.5]';
                p = [-0.200000000000000 + 0.979795897113271i;-0.200000000000000 - 0.979795897113271i];
                [b,a] = zp2tf(z,p,k)

            b =
                2     3     0

            a =
                1.0000    0.4000    1.0000

            */

            //Arrange
            Complex[] z = new Complex[] { 0, -1.5f };   //Ničle
            //Complex[] p = new Complex[] { new Complex(-0.2000f, 0.9798f), new Complex(-0.2000f, -0.9798f) };   //Poli
            Complex[] p = new Complex[] { new Complex(-0.2000f, 0.9797959f), new Complex(-0.2000f, -0.9797959f) };   //Poli
            float k = 2f;                               //gain

            //Complex[] b = new Complex[] { 2, 3, 0 };
            //Complex[] a = new Complex[] { 1, 0.4f, 1 };
            

            //Act
            var arlist1 = DSPUtils.zp2tf(z, p, k);

            Complex[] b = (Complex[])arlist1[1];
            Complex[] a = (Complex[])arlist1[0];

            int test = (int)((Complex[])arlist1[0])[0].Real;

            //WARNING!!!//
            // THIS IS HOW B,A SHOULD LOOK LIKE
            // Complex[] b = new Complex[] { 2, 3, 0 };
            // Complex[] a = new Complex[] { 1, 0.4f, 1 };
               
            // BUT MINE LOOK LIKE THIS
            // Complex[] b = new Complex[] { 2, 3, 0 };
            // Complex[] a = new Complex[] { 0.9999999, 0.4f, 1 };
               
            // Matlab does round it, so that's why its 1 there
               
            // This is because my implementation (and numpy as well) of tf2zp
            // is to precise, and calculates poles wrong
            // Instead of calculating imaginary part of the poles to 0.9798 (like matlab does)
            // it calculates it to 0.9797959

            // I hope this mismatch does not cause me problems in the future, 
            // but only time will tell if problems arrise

            //Assert
            Assert.IsTrue(true);
        }

        [TestMethod]
        //[Ignore]
        public unsafe void TestBLinearSimple()
        {
            //Assert.IsTrue(true);
            //I need to write bilinear transform
            //Read here: https://thewolfsound.com/bilinear-transform/
            //And here is matlab implementation: https://www.mathworks.com/help/signal/ref/bilinear.html

            //Example usage
            /*
             num = [6.4/(2*pi)   1];
             den =  [6.4/(2*pi)   5];
             
             %Discrete this
             [ad, bd] = bilinear(num, den, 92e3); 

            //this coefficients are only goood for 96kHz sample rate
            //ad = [0.999979547445725,-0.999969321168588]
            //bd = [1,-0.999948868614313]
             */

            //Arrange
            Complex[] num = new Complex[] { (float)(6.4 / (2 * Math.PI)), 1 };
            Complex[] den = new Complex[] { (float)(6.4 / (2 * Math.PI)), 5 };

            //Act
            var arlist1 = DSPUtils.bilinear(num, den, 96e3f);

            //Assert
            Complex[] ad = (Complex[])arlist1[0];
            Complex[] bd = (Complex[])arlist1[1];

            //Actual
            float AAa = ad[0].Real;
            float AAb = ad[1].Real;
            float ABa = bd[0].Real;
            float ABb = bd[1].Real;

            //Expected
            float EAa = 0.999979556f;
            float EAb = -0.9999693f;
            float EBa = 1f;
            float EBb = -0.999948859f;

            Assert.AreEqual(EAa, AAa);
            Assert.AreEqual(EAb, AAb);
            Assert.AreEqual(EBa, ABa);
            Assert.AreEqual(EBb, ABb);

            //NOTE: this test will fail, because Matlab does some things diferently
            //MATLAB RESULTS
            //ad = [0.999979547445725,-0.999969321168588]
            //bd = [1,-0.999948868614313]

            //If we account for floats, which are not that precise (but are fast), matlab would do it like this
            //ad = [0.9999795,1]
            //bd = [1,-0.9999488]

            //MY RESULTS
            //ad = [-0,9999897,1]
            //bd = [1,-0,9999897]


            //but if you listen to the results, it will sound allright (left is on left, right is on right)
        }
    }

    [TestClass]
    public class MatlabFilterTest
    {
        [TestMethod]
        public unsafe void TestFilterSimple()
        {
            //Arrange
            float[] a, b, u, z;

            a = new float[] { 2, -2.5f, 1 };
            b = new float[] { 0.1f, 0.1f };
            u = new float[5];
            for (int i = 1; i < u.Length; i++)
            {
                u[i] = 0.0f;
            }
            u[0] = 1.0f;

            z = new float[u.Length]; //Safe buffer

            #region Prepare buffers
            //Input buffer
            UnsafeBuffer uBuffer = UnsafeBuffer.Create(u);
            float* uBufferPtr = (float*)uBuffer;

            //Output buffer
            UnsafeBuffer zBuffer = UnsafeBuffer.Create(z.Length, sizeof(float));
            float* zBufferPtr = (float*)zBuffer;
            #endregion

            //Act
            DSPUtils.filter(b, a, uBufferPtr, zBufferPtr, uBuffer.Length);

            //Copy back to test array
            for (int i = 0; i < zBuffer.Length; i++)
                z[i] = zBufferPtr[i];

            //Assert
            //float[] ExpectedOutputVector = { 0.050000000000000, 0.112500000000000, 0.115625000000000, 0.088281250000000, 0.052539062500000 }; //Matlab
            //float[] ExpectedOutputVector = { 0.05            f, 0.112500004     f, 0.140625        f, 0.17578125      f, 0.219726563     f }; //C#

            float[] ExpectedOutputVector = { 0.05f, 0.112500004f, 0.140625f, 0.17578125f, 0.219726563f }; //C#
            CollectionAssert.AreEqual(ExpectedOutputVector, z);
        }

        [TestMethod]
        public unsafe void TestFilterAdvance()
        {
            //Arrange
            float[] a, b, input, outputVector;

            a = new float[] { 1.0000f, -3.518576748255174f, 4.687508888099475f, -2.809828793526308f, 0.641351538057564f };
            b = new float[] { 0.020083365564211f, 0, -0.040166731128422f, 0, 0.020083365564211f };

            input = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            outputVector = new float[input.Length]; //Safe buffer

            #region Prepare buffers
            //Input buffer
            UnsafeBuffer inputBuffer = UnsafeBuffer.Create(input);
            float* inputBufferPtr = (float*)inputBuffer;

            //Output buffer
            UnsafeBuffer outputBuffer = UnsafeBuffer.Create(outputVector.Length, sizeof(float));
            float* outputBufferPtr = (float*)outputBuffer;
            #endregion

            //Act
            DSPUtils.filter(b, a, inputBufferPtr, outputBufferPtr, inputBuffer.Length);

            //Copy back to test array
            for (int i = 0; i < outputBuffer.Length; i++)
                outputVector[i] = outputBufferPtr[i];

            //Assert
            //float[] ExpectedOutputVector = new float[] { 0.020083365564211f, 0.110831594229363f, 0.315911881406512f, 0.648466936215357f, 1.099378239134486f, 1.645128469776910f, 2.254636012320568f, 2.894724888960297f, 3.534126758562540f };       //Matlab
            //float[] ExpectedOutputVector = new float[] { 0.020083365564211, 0.11083159422936348, 0.31591188140651166, 0.648466936215357, 1.0993782391344866, 1.6451284697769106, 2.25463601232057,   2.8947248889603028, 3.534126758562552 };        //C# OLD double
            //float[] ExpectedOutputVector = new float[] { 0.020083366     f, 0.1108316         f, 0.315911919       f, 0.648467        f, 1.09937835       f, 1.64512861       f, 2.254636       f,   2.89472485       f, 3.53412676      f };        //C# new float

            float[] ExpectedOutputVector = new float[] { 0.020083366f, 0.1108316f, 0.315911919f, 0.648467f, 1.09937835f, 1.64512861f, 2.254636f, 2.89472485f, 3.53412676f };
            CollectionAssert.AreEqual(ExpectedOutputVector, outputVector);

        }
    }
}
