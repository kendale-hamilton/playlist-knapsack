using System.Numerics;
using System.Text;
using System.Text.Json;
using Models.Knapsack;
using Models.Requests.Knapsack;
using Services.BlobService;

namespace Services.KnapsackService
{
    public class KnapsackService : IKnapsackService
    {
        private readonly IBlobService _blobService;
        public KnapsackService(IBlobService blobService)
        {
            _blobService = blobService;
        }
        public async Task<List<Track>> GetSolvedPlaylist(string userId, string customId)
        {
            string blob = $"{userId}/{customId}.json";
            List<Track> tracks = await _blobService.DownloadFile<List<Track>>(blob);
            //TODO: Think about better way to delete after
            await _blobService.DeleteFile(blob);
            return tracks;
        }

        public async Task<string> SolveKnapsack(DesiredLengths desiredLengths, List<Track> tracks, string userId)
        {
            SubsetNode[] nodes = new SubsetNode[tracks.Count];
            for (int i = 0; i < tracks.Count; i++)
            {
                nodes[i] = new SubsetNode(tracks[i]);
            }
            
            SubsetNode[] level = nodes;
            while (level.Length > 1)
            {
                SubsetNode[] nextLevel = new SubsetNode[(level.Length + 1) / 2];
                for (int i = 0, j = 0; j < level.Length; i+=1, j+=2)
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
            SubsetNode top = level[0];
            top.Vector.Print("Top Vector: ");

            int length = desiredLengths.Length;
            int max = desiredLengths.Max ?? 0;
            int min = desiredLengths.Min ?? 0;
            int foundTotal = 0;

            if (!top.Vector.ContainsValue(length) && (max == 0) && (min == 0))
            {
                throw new Exception("No solution found");
            }
            else if (top.Vector.ContainsValue(length))
            {
                foundTotal = length;
            }
            else if (max != 0 && min != 0)
            {
                for (int i = 0; i + length <= max || length - i >= min; i++)
                {
                    if (length + i <= max && top.Vector.ContainsValue(length + i))
                    {
                        foundTotal = length + 1;
                        break;
                    }
                    else if (length + i >= min && top.Vector.ContainsValue(length - i))
                    {
                        foundTotal = length - i;
                        break;
                    }
                }
            }
            else
            {
                throw new Exception("No solution found");
            }

            Vec total = new Vec(foundTotal, 1);
            List<Track> selections = BackwardsPass(total, top);

            string json = JsonSerializer.Serialize(selections);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            Guid guid = Guid.NewGuid();

            string filename = $"{userId}/{guid}.json";
            await _blobService.UploadFile(filename, stream);

            return guid.ToString();
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
            Vec reversed = rhs.DeepCopy();
            reversed.Reverse();
            Vec convolve = FFTConvolve(lhs, reversed);
            int shiftValue = rhs.Length - 1; 
            return convolve.Shift(shiftValue);
        }

        private static Vec FFTConvolve(Vec lhs, Vec rhs)
        {
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
        private List<Track> BackwardsPass(Vec sum, SubsetNode parent)
        {
            if (parent.LeftChild == null && parent.RightChild == null)
            {
                if (sum.IsZero())
                {
                    return [];
                }
                else
                {
                    return [parent.Track];
                }
            }
            SubsetNode left = parent.LeftChild;
            SubsetNode right = parent.RightChild;

            Vec leftoverLeft = VectorSubtraction(sum, right.Vector);
            Vec leftoverRight = VectorSubtraction(sum, left.Vector);

            int lMatch = 0;
            int rMatch = 0;
            while (lMatch + rMatch != sum.Length - 1)
            {
                lMatch = left.Vector.FirstMatch(leftoverLeft);
                if (leftoverRight.ContainsValue(sum.Length - 1 - lMatch) && right.Vector.ContainsValue(sum.Length - 1 - lMatch))
                {
                    rMatch = sum.Length - 1 - lMatch;
                    break;
                }
                left.Vector.Pop(lMatch);
            }
            
            Vec lMatchVec = new Vec(lMatch, 1);
            Vec rMatchVec = new Vec(rMatch, 1);


            List<Track> leftPass = BackwardsPass(lMatchVec, left);
            List<Track> rightPass = BackwardsPass(rMatchVec, right);
            List<Track> children = [.. leftPass, .. rightPass];
            return children;
        }
        //Used for checking accuracy of FFTConvolve
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