using System.Numerics;
using Models.Knapsack;

namespace Services.KnapsackService
{
    public class KnapsackService : IKnapsackService
    {
        public KnapsackService(){}
        public Task<List<Track>> SolveKnapsack(int length, List<Track> tracks)
        {
            double[] values = new double[tracks.Count];
            for (int i = 0; i < tracks.Count; i++)
            {
                values[i] = (double)tracks[i].DurationMs;
            }

            Vec[] vectors = new Vec[tracks.Count];

            for (int i = 0; i < tracks.Count; i++)
            {
                int size = (int)tracks[i].DurationMs;
                Vec vector = new Vec(size + 1);
                vector[0] = 1;
                vector[size] = 1;
                vectors[i] = vector;
            }
            
            Console.WriteLine("Starting FFT Convolution");
            while (vectors.Length > 1)
            {
                for (int i = 0, j = 0; j < vectors.Length - 1; i += 1, j += 2)
                {
                    if (j + 1 < vectors.Length)
                    {
                        vectors[i] = FFTConvolve(vectors[j], vectors[j + 1]);
                    }
                    else
                    {
                        vectors[i] = vectors[j];
                    }
                }
                vectors = new ArraySegment<Vec>(vectors, 0, (vectors.Length + 1) / 2).ToArray();
            }
            Vec result = vectors[0];
            Console.WriteLine("Finished FFT Convolution");
            
            result.Print("Final FFT Result: ");

            return Task.FromResult(tracks);
        }

        private static void FFT(Vec vector)
        {
            int n = vector.Length;
            if (n == 1)
            {
                return;
            }
            if (n % 2 != 0)
            {
                throw new ArgumentException("Vector length must be a power of 2");
            }
            Vec packedEvens = new Vec(n/2);
            Vec packedOdds = new Vec(n/2);
            for (int i = 0; i < n / 2; i++)
            {
                packedEvens[i] = vector[2 * i];
                packedOdds[i] = vector[2 * i + 1];
            }

            FFT(packedEvens);
            FFT(packedOdds);
            for (int i = 0; i < n / 2; i++)
            {
                Complex x = Complex.Exp(-2 * Math.PI * i * Complex.ImaginaryOne / n);
                vector[i] = packedEvens[i] + x * packedOdds[i];
                vector[i + n / 2] = packedEvens[i] - x * packedOdds[i];
            }
        }

        private Vec FFTConvolve(Vec lhs, Vec rhs)
        {
            // Console.WriteLine($"Convolving lhs: {string.Join(", ", lhs)} and rhs: {string.Join(", ", rhs)}");

            int size = Math.Max(lhs.Length, rhs.Length);
            int n = 1;
            while (n < size) n *= 2;

            lhs = lhs.Pad(n * 2);
            rhs = rhs.Pad(n * 2);
            
            FFT(lhs);
            FFT(rhs);

            Vec result = new Vec(2 * n);
            for (int i = 0; i < 2 * n; i++)
            {
                Complex product = lhs[i] * rhs[i];
                result[i] = product;
            }
            for (int i = 0; i < 2 * n; i++)
            {
                result[i] = Complex.Conjugate(result[i] / (2 * n));
            }
            FFT(result);
            Vec resultSegment = result.Slice(0, 2 * n - 1);
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