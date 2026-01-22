using System.Numerics;
using System.Text.Json;
using Tutor.Services.Logging;

namespace Tutor.Services;

/// <summary>
/// Locality-Sensitive Hashing service using random hyperplane projections.
/// Produces compact binary signatures from embedding vectors for fast approximate nearest neighbor search.
/// </summary>
public sealed class LSHService
{
    private const string HyperplanesFileName = "lsh_hyperplanes.json";
    private const int DefaultBitCount = 256;
    private const int DefaultSeed = 1337;

    private readonly object lockObj = new();

    private float[][]? hyperplanes;

    /// <summary>
    /// Number of bits in the signature (number of hyperplanes).
    /// </summary>
    public int BitCount { get; }

    /// <summary>
    /// Dimension of the embedding vectors.
    /// </summary>
    public int EmbeddingDimension { get; }

    /// <summary>
    /// Random seed used for hyperplane generation.
    /// </summary>
    public int Seed { get; }

    /// <summary>
    /// Gets the current hyperplanes file path (uses DataStorageSettings).
    /// </summary>
    private string HyperplanesFilePath => Path.Combine(DataStorageSettings.GetLSHDirectory(), HyperplanesFileName);

    /// <summary>
    /// Creates a new LSH service with the specified parameters.
    /// </summary>
    /// <param name="embeddingDimension">Dimension of embedding vectors (default 1536 for OpenAI text-embedding-3-small).</param>
    /// <param name="bitCount">Number of bits in the signature (default 256).</param>
    /// <param name="seed">Random seed for deterministic hyperplane generation (default 1337).</param>
    public LSHService(int embeddingDimension = 1536, int bitCount = DefaultBitCount, int seed = DefaultSeed)
    {
        EmbeddingDimension = embeddingDimension;
        BitCount = bitCount;
        Seed = seed;
    }

    /// <summary>
    /// Computes a binary signature from an embedding vector using random hyperplane projections.
    /// Each bit indicates which side of a hyperplane the embedding falls on.
    /// </summary>
    /// <param name="embedding">The embedding vector to compute the signature for.</param>
    /// <returns>A packed byte array containing the binary signature. Length is (BitCount + 7) / 8.</returns>
    public byte[] GetSignature(float[] embedding)
    {
        if (embedding == null || embedding.Length == 0)
            return [];

        EnsureHyperplanesLoaded();

        var signatureBytes = (BitCount + 7) / 8;
        var signature = new byte[signatureBytes];

        for (int i = 0; i < BitCount; i++)
        {
            var plane = hyperplanes![i];
            var dot = DotProduct(embedding, plane);

            // If dot product >= 0, set bit to 1 (LSB first within each byte)
            if (dot >= 0)
            {
                var byteIndex = i / 8;
                var bitIndex = i % 8;
                signature[byteIndex] |= (byte)(1 << bitIndex);
            }
        }

        return signature;
    }

    /// <summary>
    /// Computes the Hamming distance between two binary signatures.
    /// The Hamming distance is the number of bit positions that differ.
    /// </summary>
    /// <param name="a">First signature.</param>
    /// <param name="b">Second signature.</param>
    /// <returns>Number of differing bits.</returns>
    public static int HammingDistance(byte[] a, byte[] b)
    {
        if (a == null || b == null)
            return int.MaxValue;

        var minLen = Math.Min(a.Length, b.Length);
        var maxLen = Math.Max(a.Length, b.Length);
        var distance = 0;

        // Count differing bits in common portion
        for (int i = 0; i < minLen; i++)
        {
            distance += PopCount((byte)(a[i] ^ b[i]));
        }

        // Any extra bytes in the longer array count as all differing bits
        distance += (maxLen - minLen) * 8;

        return distance;
    }

    /// <summary>
    /// Counts the number of set bits (1s) in a byte using hardware intrinsics when available.
    /// </summary>
    private static int PopCount(byte value)
    {
        return BitOperations.PopCount(value);
    }

    /// <summary>
    /// Computes the dot product of two vectors.
    /// </summary>
    private static float DotProduct(float[] a, float[] b)
    {
        var len = Math.Min(a.Length, b.Length);
        float sum = 0;
        for (int i = 0; i < len; i++)
        {
            sum += a[i] * b[i];
        }
        return sum;
    }

    /// <summary>
    /// Ensures hyperplanes are loaded from disk or generated.
    /// </summary>
    private void EnsureHyperplanesLoaded()
    {
        if (hyperplanes != null)
            return;

        lock (lockObj)
        {
            if (hyperplanes != null)
                return;

            // Try to load from disk first
            if (TryLoadHyperplanes())
                return;

            // Generate new hyperplanes
            GenerateHyperplanes();

            // Persist for stability across sessions
            SaveHyperplanes();
        }
    }


    /// <summary>
    /// Attempts to load hyperplanes from the persisted file.
    /// </summary>
    private bool TryLoadHyperplanes()
    {
        try
        {
            if (!File.Exists(HyperplanesFilePath))
                return false;

            var json = File.ReadAllText(HyperplanesFilePath);
            var data = JsonSerializer.Deserialize<HyperplaneData>(json);

            if (data == null ||
                data.BitCount != BitCount ||
                data.EmbeddingDimension != EmbeddingDimension ||
                data.Seed != Seed ||
                data.Hyperplanes == null ||
                data.Hyperplanes.Length != BitCount)
            {
                // Mismatch in parameters, regenerate
                Log.Debug("LSH: Hyperplane parameters changed, regenerating");
                return false;
            }

            Log.Debug($"LSH: Loaded {BitCount} hyperplanes from disk");
            hyperplanes = data.Hyperplanes;
            return true;
        }
        catch (Exception ex)
        {
            Log.Warn($"LSH: Failed to load hyperplanes - {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Generates random hyperplanes using a deterministic PRNG.
    /// Uses Box-Muller transform to generate Gaussian-distributed values.
    /// </summary>
    private void GenerateHyperplanes()
    {
        Log.Debug($"LSH: Generating {BitCount} hyperplanes for {EmbeddingDimension}-dimensional space (seed={Seed})");
        var rng = new Random(Seed);
        hyperplanes = new float[BitCount][];

        for (int i = 0; i < BitCount; i++)
        {
            var plane = new float[EmbeddingDimension];

            // Generate Gaussian-distributed random values using Box-Muller
            for (int j = 0; j < EmbeddingDimension; j += 2)
            {
                var (g1, g2) = BoxMuller(rng);
                plane[j] = g1;
                if (j + 1 < EmbeddingDimension)
                {
                    plane[j + 1] = g2;
                }
            }

            // Normalize the plane to unit length
            NormalizeVector(plane);

            hyperplanes[i] = plane;
        }
    }

    /// <summary>
    /// Box-Muller transform to generate two independent standard normal random variables.
    /// </summary>
    private static (float, float) BoxMuller(Random rng)
    {
        double u1, u2;
        do
        {
            u1 = rng.NextDouble();
            u2 = rng.NextDouble();
        } while (u1 <= double.Epsilon);

        var mag = Math.Sqrt(-2.0 * Math.Log(u1));
        var angle = 2.0 * Math.PI * u2;

        return ((float)(mag * Math.Cos(angle)), (float)(mag * Math.Sin(angle)));
    }

    /// <summary>
    /// Normalizes a vector to unit length in place.
    /// </summary>
    private static void NormalizeVector(float[] vector)
    {
        float sumSq = 0;
        for (int i = 0; i < vector.Length; i++)
        {
            sumSq += vector[i] * vector[i];
        }

        if (sumSq > 0)
        {
            var norm = (float)Math.Sqrt(sumSq);
            for (int i = 0; i < vector.Length; i++)
            {
                vector[i] /= norm;
            }
        }
    }

    /// <summary>
    /// Saves hyperplanes to disk for persistence.
    /// </summary>
    private void SaveHyperplanes()
    {
        try
        {
            var data = new HyperplaneData
            {
                BitCount = BitCount,
                EmbeddingDimension = EmbeddingDimension,
                Seed = Seed,
                Hyperplanes = hyperplanes!
            };

            var json = JsonSerializer.Serialize(data);
            File.WriteAllText(HyperplanesFilePath, json);
            Log.Debug($"LSH: Saved {BitCount} hyperplanes to disk");
        }
        catch (Exception ex)
        {
            Log.Warn($"LSH: Failed to save hyperplanes - {ex.Message}");
            // Hyperplanes are still in memory, continue operation
        }
    }

    /// <summary>
    /// Data structure for persisting hyperplanes.
    /// </summary>
    private sealed class HyperplaneData
    {
        public int BitCount { get; set; }
        public int EmbeddingDimension { get; set; }
        public int Seed { get; set; }
        public float[][] Hyperplanes { get; set; } = [];
    }
}
