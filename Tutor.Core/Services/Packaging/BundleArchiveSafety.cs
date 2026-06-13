using System.Security.Cryptography;
using System.Text;

namespace Tutor.Core.Services.Packaging;

/// <summary>
/// Pure helpers that treat a .tutor file as untrusted input: zip-slip entry-path
/// screening and the canonical payload hash used for integrity checks.
/// </summary>
public static class BundleArchiveSafety
{
    /// <summary>
    /// True when a zip entry path is safe to honor: relative, forward-slash only,
    /// no drive letters, no rooted paths, and no <c>..</c> escapes. Tutor never
    /// extracts entries to disk by path today, but every reader screens names
    /// anyway so the guard is already in place the day bundles carry assets.
    /// </summary>
    public static bool IsSafeEntryPath(string entryName)
    {
        if (string.IsNullOrWhiteSpace(entryName)) return false;
        if (entryName.Contains('\\')) return false;
        if (entryName.StartsWith('/')) return false;
        if (entryName.Contains(':')) return false;          // drive letters, NTFS streams
        if (entryName.Contains('\0')) return false;

        foreach (var segment in entryName.Split('/'))
        {
            if (segment.Length == 0) return false;           // "//" or trailing "/"
            if (segment == "." || segment == "..") return false;
        }

        return true;
    }

    /// <summary>
    /// Canonical SHA-256 (lowercase hex) over bundle payload entries. Entries are
    /// hashed sorted by name (ordinal) so the digest is independent of zip write
    /// order: for each entry — UTF-8 name, a zero byte, the content length as
    /// 8 little-endian bytes, then the content itself.
    /// </summary>
    public static string ComputePayloadSha256(IEnumerable<(string Name, byte[] Content)> entries)
    {
        using var sha = SHA256.Create();

        foreach (var (name, content) in entries.OrderBy(e => e.Name, StringComparer.Ordinal))
        {
            var nameBytes = Encoding.UTF8.GetBytes(name);
            sha.TransformBlock(nameBytes, 0, nameBytes.Length, null, 0);

            var separator = new byte[1];
            sha.TransformBlock(separator, 0, 1, null, 0);

            var length = BitConverter.GetBytes((long)content.Length);
            if (!BitConverter.IsLittleEndian) Array.Reverse(length);
            sha.TransformBlock(length, 0, length.Length, null, 0);

            sha.TransformBlock(content, 0, content.Length, null, 0);
        }

        sha.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        return Convert.ToHexString(sha.Hash!).ToLowerInvariant();
    }
}
