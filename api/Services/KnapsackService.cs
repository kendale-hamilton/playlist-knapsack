
using System.Numerics;
using Models.Knapsack;

namespace Services.KnapsackService
{
    public class KnapsackService : IKnapsackService
    {
        public KnapsackService(){}
        public Task<List<Track>> SolveKnapsack(int length, List<Track> tracks)
        {
            double[] vec1 = new double[] {1, 2, 3, 4, 5};
            double[] vec2 = new double[] {6, 4, 8, 7, 9};
            double[] result = FFTConvolve(vec1, vec2);
            Console.WriteLine("FFT Result: " + string.Join(", ", result));
            double[] naiveResult = StandardConvolve(vec1, vec2);
            Console.WriteLine("Naive Result: " + string.Join(", ", naiveResult));
            // double[] values = new double[tracks.Count];
            // for (int i = 0; i < tracks.Count; i++)
            // {
            //     values[i] = (double)tracks[i].DurationMs;
            // }

            // double[][] arrays = new double[tracks.Count][];

            // for (int i = 0; i < tracks.Count; i++)
            // {
            //     int size = (int)tracks[i].DurationMs;
            //     double[] array = new double[size + 1];
            //     array[0] = 1;
            //     array[size] = 1;
            //     arrays[i] = array;
            // }
            
            // Console.WriteLine("Starting FFT Convolution");
            // while (arrays.Length > 1)
            // {
            //     for (int i = 0, j = 0; j < arrays.Length - 1; i += 1, j += 2)
            //     {
            //         if (j + 1 < arrays.Length)
            //         {
            //             arrays[i] = FFTConvolve(arrays[j], arrays[j + 1]);
            //         }
            //         else
            //         {
            //             arrays[i] = arrays[j];
            //         }
            //     }
            //     arrays = new ArraySegment<double[]>(arrays, 0, (arrays.Length + 1) / 2).ToArray();
            // }

            // Console.WriteLine("Finished FFT Convolution");
            // // Console.WriteLine("Final FFT Convolution Result: " + string.Join(", ", arrays[0]));



            return Task.FromResult(tracks);
        }

        private static Complex[] FFT(double[] vector)
        {
            Complex[] complexVec = new Complex[vector.Length];
            for (int i = 0; i < vector.Length; i++)
            {
                complexVec[i] = new Complex(vector[i], 0);
            }
            FFT(complexVec);
            return complexVec;
        }

        private static void FFT(Complex[] vector)
        {
            int n = vector.Length;
            if (n <= 1)
            {
                return;
            }

            Complex[] packedEvens = new Complex[n/2];
            Complex[] packedOdd = new Complex[n/2];
            for (int i = 0; i < n/2; i++)
            {
                packedEvens[i] = vector[2 * i];
                packedOdd[i] = vector[2 * i + 1];
            }

            FFT(packedEvens);
            FFT(packedOdd);
            
            for (int i = 0; i < n / 2; i++)
            {
                Complex x = Complex.Exp(-2 * Math.PI * i * Complex.ImaginaryOne / n);
                vector[i] = packedEvens[i] + x * packedOdd[i];
                vector[i + n / 2] = packedEvens[i] - x * packedOdd[i];
            }
        }

        private double[] FFTConvolve(Complex[] lhs, Complex[] rhs)
        {
            int n = lhs.Length;
            double[] realLhs = new double[n];
            double[] realRhs = new double[n];
            for (int i = 0; i < n; i++)
            {
                realLhs[i] = lhs[i].Real;
                realRhs[i] = rhs[i].Real;
            }
            return FFTConvolve(realLhs, realRhs);
        }

        private double[] FFTConvolve(double[] lhs, double[] rhs)
        {
            Console.WriteLine($"Convolving lhs: {string.Join(", ", lhs)} and rhs: {string.Join(", ", rhs)}");

            int n;
            if (lhs.Length > rhs.Length)
            {
                n = lhs.Length;
                rhs = Pad(rhs, n);
            }
            else
            {
                n = rhs.Length;
                lhs = Pad(lhs, n);
            }
            double[] lhsPad = new double[2 * n];
            double[] rhsPad = new double[2 * n];
            for (int i = 0; i < n; i++)
            {
                lhsPad[i] = lhs[i];
                rhsPad[i] = rhs[i];
            }

            Complex[] lhsComplex = FFT(lhsPad);
            Complex[] rhsComplex = FFT(rhsPad);

            Complex[] complexResult = new Complex[2 * n];
            double[] result = new double[2 * n];
            for (int i = 0; i < 2 * n; i++)
            {
                Complex product = lhsComplex[i] * rhsComplex[i];
                Console.WriteLine($"Product {i}: {product}");
                complexResult[i] = Complex.Conjugate(product);
            }
            FFT(complexResult);
            Console.WriteLine($"Complex Result: {string.Join(", ", complexResult)}");
            for (int i = 0; i < 2 * n; i++)
            {
                complexResult[i] = Complex.Conjugate(complexResult[i] / (2 * n));
                result[i] = complexResult[i].Real;
            }
            double[] resultSegment = new ArraySegment<double>(result, 0, (n * 2) - 1).ToArray();
            Console.WriteLine($"Result: {string.Join(", ", resultSegment)}");
            return resultSegment;
        }

        private double MaxConvolution(int lhs, int rhs)
        {
            int p = 16;
            double powered_lhs = Math.Pow(lhs, p);
            // Console.WriteLine("Powered LHS: " + powered_lhs);
            double powered_rhs = Math.Pow(rhs, p);
            // Console.WriteLine("Powered RHS: " + powered_rhs);
            double result = Math.Pow(powered_lhs + powered_rhs, 1.0 / p);
            // Console.WriteLine("Result: " + result);
            return result;
        }

        private static T[] Pad<T>(T[] array, int length)
        {
            T[] paddedArray = new T[length];
            for (int i = 0; i < array.Length; i++)
            {
                paddedArray[i] = array[i];
            }
            return paddedArray;
        }
        //TODO: Remove
        private static double[] StandardConvolve(double[] lhs, double[] rhs)
        {
            int n = lhs.Length;
            double[] result = new double[2 * n - 1];
            for (int i = 0; i < 2 * n - 1; i++)
            {
                double sum = 0;
                for (int j = 0; j <= i; j++)
                {
                    if (j < n && i - j < n)
                    {
                        sum += lhs[j] * rhs[i - j];
                    }
                }
                result[i] = sum;
            }
            return result;
        }
    }
}