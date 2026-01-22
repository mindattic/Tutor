using System.Numerics;
using Tutor.Services.Logging;

namespace Tutor.Services;

/// <summary>
/// SimHash service for computing locality-sensitive lexical fingerprints.
/// Produces compact 64-bit signatures where similar text has similar signatures.
/// Uses FNV-1a hashing for deterministic, stable results across sessions.
/// </summary>
public sealed class SimHashService
{
    /// <summary>
    /// Number of bits in the signature.
    /// </summary>
    public const int BitCount = 64;

    // FNV-1a 64-bit constants
    private const ulong FnvOffsetBasis = 14695981039346656037UL;
    private const ulong FnvPrime = 1099511628211UL;

    /// <summary>
    /// Computes a 64-bit SimHash signature for the given text.
    /// Similar text will produce signatures with low Hamming distance.
    /// </summary>
    /// <param name="text">The text to hash.</param>
    /// <returns>A 64-bit SimHash signature.</returns>
    public ulong GetSignature64(string text)
    {
        if (string.IsNullOrEmpty(text))
            return 0UL;

        // Accumulator for each bit position
        var v = new int[BitCount];

        // Tokenize and process
        var tokens = Tokenize(text);

        foreach (var token in tokens)
        {
            // Hash the token using FNV-1a 64-bit
            var hash = Fnv1a64(token);

            // For each bit, add weight if set, subtract if not
            for (int i = 0; i < BitCount; i++)
            {
                var bit = (hash >> i) & 1UL;
                if (bit == 1)
                    v[i]++;
                else
                    v[i]--;
            }
        }

        // Build final signature: bit i is 1 if v[i] >= 0
        ulong signature = 0UL;
        for (int i = 0; i < BitCount; i++)
        {
            if (v[i] >= 0)
            {
                signature |= (1UL << i);
            }
        }

        return signature;
    }

    /// <summary>
    /// Computes the Hamming distance between two 64-bit signatures.
    /// The Hamming distance is the number of bit positions that differ.
    /// </summary>
    /// <param name="a">First signature.</param>
    /// <param name="b">Second signature.</param>
    /// <returns>Number of differing bits (0 to 64).</returns>
    public static int HammingDistance(ulong a, ulong b)
    {
        return PopCount(a ^ b);
    }

    /// <summary>
    /// Counts the number of set bits in a 64-bit value using hardware intrinsics when available.
    /// </summary>
    private static int PopCount(ulong value)
    {
        return BitOperations.PopCount(value);
    }

    /// <summary>
    /// Tokenizes text into words for SimHash computation.
    /// </summary>
    private static List<string> Tokenize(string text)
    {
        var tokens = new List<string>();
        var current = new System.Text.StringBuilder();

        // Convert to lowercase for normalization
        text = text.ToLowerInvariant();

        foreach (var c in text)
        {
            if (char.IsLetterOrDigit(c))
            {
                current.Append(c);
            }
            else
            {
                // Non-alphanumeric character is a separator
                if (current.Length > 0)
                {
                    tokens.Add(current.ToString());
                    current.Clear();
                }
            }
        }

        // Add final token if any
        if (current.Length > 0)
        {
            tokens.Add(current.ToString());
        }

        return tokens;
    }

    /// <summary>
    /// FNV-1a 64-bit hash function.
    /// A simple, fast, and deterministic hash algorithm.
    /// </summary>
    private static ulong Fnv1a64(string input)
    {
        var hash = FnvOffsetBasis;

        foreach (var c in input)
        {
            hash ^= c;
            hash *= FnvPrime;
        }

        return hash;
    }
}
