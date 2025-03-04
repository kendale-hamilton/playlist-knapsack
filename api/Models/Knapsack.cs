using System.Collections;
using System.Numerics;
using System.Text.Json.Serialization;
using Models.Spotify;

namespace Models.Knapsack
{
    #region Playlist
    public class Playlist
    {
        [JsonPropertyName("details")]
        public PlaylistDetails Details { get; set; }
        [JsonPropertyName("tracks")]
        public List<Track> Tracks { get; set; }
    }

    public class PlaylistDetails
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("images")]
        public List<SpotifyImage>? Images { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; } 
        [JsonPropertyName("duration_ms")]
        public int DurationMs { get; set; }
    }
    #endregion
    #region Track
    public class Track
    {
        [JsonPropertyName("album")]
        public SpotifyAlbum? Album { get; set; }
        [JsonPropertyName("artists")]
        public List<SpotifyArtist>? Artists { get; set; }
        [JsonPropertyName("duration_ms")]
        public int? DurationMs { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("popularity")]
        public int? Popularity { get; set; }
    }
    #endregion

    #region Solver
    public class Vec : IEnumerable<Complex>
    {
        private readonly Complex[] _values;
        public int Length => _values.Length;
        public Vec(int size)
        {
            _values = new Complex[size];
        }
        public Vec(IEnumerable<Complex> values)
        {
            _values = values.ToArray();
        }

        public Vec(IEnumerable<double> values)
        {
            _values = values.Select(x => new Complex(x, 0)).ToArray();
        }

        public Vec(int size, double value)
        {
            _values = new Complex[size + 1];
            _values[size] = new Complex(value, 0);
        }

        public Complex this[int index]
        {
            get => _values[index];
            set => _values[index] = value;
        }

        public IEnumerator<Complex> GetEnumerator()
        {
            return ((IEnumerable<Complex>)_values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Complex>)_values).GetEnumerator();
        }

        public void Print(string prepend = "")
        {
            List<int> nonZeroIndexes = GetNonZeroIndexes();
            Console.WriteLine(prepend + string.Join(", ", nonZeroIndexes));
        }

        public void PrintReal(string prepend = "")
        {
            List<double> reals = new List<double>();
            for (int i = 0; i < _values.Length; i++)
            {
                double value = _values[i].Real;
                if (value > .1 || value < -.1)
                {
                    reals.Add(value);
                }
                else 
                {
                    reals.Add(0);
                }
            }
            Console.WriteLine(prepend + string.Join(", ", reals));
        }

        public void PrintComplex(string prepend = "")
        {
            Console.WriteLine(prepend + string.Join(", ", _values));
        }

        public Vec Pad(int size)
        {
            Vec padded = new Vec(size);
            for (int i = 0; i < _values.Length; i++)
            {
                padded[i] = _values[i];
            }
            return padded;
        }

        public Vec Slice(int start, int end)
        {
            Vec sliced = new Vec(end - start);
            for (int i = start; i < end; i++)
            {
                sliced[i - start] = _values[i];
            }
            return sliced;
        }

        public Vec Trim()
        {
            int i = _values.Length - 1;
            while (i >= 0 && _values[i].Magnitude < .1)
            {
                i--;
            }
            return Slice(0, i + 1);
        }

        public void Reverse()
        {
            Array.Reverse(_values);
        }

        public Vec Shift(int amount)
        {
            // Print("Pre Shifting Original:");
            // PrintReal("Pre Shifting Original:");
            // Console.WriteLine($"Shifting by {amount}");
            Vec shifted = new Vec(_values.Length - amount);
            // shifted.PrintReal("Pre Shifting Shifted:");
            for (int i = 0; i < shifted.Length; i++)
            {
                shifted[i] = _values[i + amount];
            }
            // shifted.PrintReal("Post Shifting Shifted:");
            return shifted;
        }
        public int Min()
        {
            for (int i = 0; i < _values.Length; i++)
            {
                if (_values[i].Real > .1)
                {
                    return i;
                }
            }
            return -1;
        }

        public List<int> GetNonZeroIndexes()
        {
            List<int> nonzeroIndexes = new List<int>();
            for (int i = 0; i < _values.Length; i++)
            {
                if (_values[i].Real > .1)
                {
                    nonzeroIndexes.Add(i);
                }
            }
            return nonzeroIndexes;
        }

        public bool ContainsValue(int value)
        {
            if (_values[value].Real > .1)
            {
                return true;
            }
            return false;
        }

        public int FirstMatch(Vec vector)
        {
            List<int> selfIndexes = GetNonZeroIndexes();
            List<int> otherIndexes = vector.GetNonZeroIndexes();
            foreach (int selfIndex in selfIndexes)
            {
                foreach (int otherIndex in otherIndexes)
                {
                    if (selfIndex == otherIndex)
                    {
                        return selfIndex;
                    }
                }
            }
            return -1;
        }

        public Vec DeepCopy()
        {
            Vec copy = new Vec(Length);
            for (int i = 0; i < Length; i++)
            {
                copy[i] = _values[i];
            }
            return copy;
        }
    }

    public class SubsetNode
    {
        public Vec Vector;
        public SubsetNode Parent;
        public SubsetNode LeftChild;
        public SubsetNode RightChild;
        public SubsetNode(Vec vec)
        {
            Vector = vec;
        }
        public SubsetNode(Vec vec, SubsetNode left, SubsetNode right)
        {
            Vector = vec;
            LeftChild = left;
            RightChild = right;
        }
    }

    #endregion
}