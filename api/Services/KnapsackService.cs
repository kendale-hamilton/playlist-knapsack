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

            SubsetNode[] vectors = new SubsetNode[tracks.Count];
            for (int i = 0; i < tracks.Count; i++)
            {
                int size = (int)tracks[i].DurationMs;
                Vec vector = new Vec(size + 1);
                vector[0] = 1;
                vector[size] = 1;
                vectors[i] = new SubsetNode(vector);
            }

            
            Console.WriteLine("Starting FFT Convolution");
            SubsetNode[] level = vectors;
            while (level.Length > 1)
            {
                SubsetNode[] nextLevel = new SubsetNode[(level.Length + 1) / 2];
                for (int i = 0, j = 0; j < level.Length - 1; i+=1, j+=2)
                {
                    if (j + 1 < level.Length)
                    {
                        SubsetNode left = level[j];
                        SubsetNode right = level[j + 1];
                        Vec sum = FFTConvolve(left.Vector, right.Vector);
                        SubsetNode node = new SubsetNode(sum, left, right);
                        nextLevel[i] = node;
                    }
                    else
                    {
                        SubsetNode child = level[j];
                        nextLevel[i] = child;
                    }
                }
                level = nextLevel;
            }   

            Console.WriteLine("Finished FFT Convolution");

            Console.WriteLine("Starting Backwards Pass");
            SubsetNode top = level[0];

            if (!top.Vector.ContainsValue(length))
            {
                Console.WriteLine("No solution found");
                return Task.FromResult(new List<Track>());
            }
            Vec total = new Vec(length, 1);

            List<Vec> selections = BackwardsPass(total, top.LeftChild, top.RightChild);
            Console.WriteLine("Finished Backwards Pass");
            foreach (Vec selection in selections)
            {
                selection.Print("Selection: ");
            }

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

        private Vec VectorSubtraction(Vec lhs, Vec rhs)
        {
            // Console.WriteLine("Subtracting vectors");
            Vec reversed = rhs.DeepCopy();
            reversed.Reverse();
            Vec convolve = FFTConvolve(lhs, reversed);
            int shiftValue = rhs.Length - 1; 
            return convolve.Shift(shiftValue);
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
        private List<Vec> BackwardsPass(Vec sum, SubsetNode left, SubsetNode right)
        {
            if (left == null && right == null)
            {
                return [sum];
            }
            Vec leftoverLeft = VectorSubtraction(sum, right.Vector);
            Vec leftoverRight = VectorSubtraction(sum, left.Vector);

            List<Vec> leftPass;
            List<Vec> rightPass;
            int lMatch = left.Vector.FirstMatch(leftoverLeft);
            Vec lMatchVec = new Vec(lMatch, 1);
            leftPass = BackwardsPass(lMatchVec, left.LeftChild, left.RightChild);
            int rMatch = right.Vector.FirstMatch(leftoverRight);
            Vec rMatchVec = new Vec(rMatch, 1);
            rightPass = BackwardsPass(rMatchVec, right.LeftChild, right.RightChild);
            List<Vec> children = [.. leftPass, .. rightPass];
            return children;
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