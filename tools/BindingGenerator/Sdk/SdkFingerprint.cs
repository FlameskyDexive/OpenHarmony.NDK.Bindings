using System.Security.Cryptography;
using System.Text;
using OpenHarmony.Ndk.Bindings.Generator.Inventory;

namespace OpenHarmony.Ndk.Bindings.Generator.Sdk;

public static class SdkFingerprint
{
    public static string ComputeFileSha256(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        using FileStream stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }

    public static string ComputeSysrootSha256(string sysroot)
    {
        return ComputeTreeSha256(sysroot, headersOnly: false);
    }

    public static string ComputeHeaderInventorySha256(string sysroot, int apiLevel)
    {
        HeaderInventoryDocument inventory = HeaderInventory.Scan(sysroot, apiLevel);
        using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        foreach (HeaderRecord header in inventory.Headers.OrderBy(header => header.Path, StringComparer.Ordinal))
        {
            AppendUtf8(hash, $"{header.Path}\0{header.Size}\0{header.Sha256.ToLowerInvariant()}\n");
        }

        return Convert.ToHexString(hash.GetHashAndReset());
    }

    private static string ComputeTreeSha256(string rootPath, bool headersOnly)
    {
        string fullRoot = Path.GetFullPath(rootPath);
        string hashRoot = headersOnly ? Path.Combine(fullRoot, "usr", "include") : fullRoot;
        if (!Directory.Exists(hashRoot))
        {
            throw new DirectoryNotFoundException($"SDK fingerprint root was not found: {hashRoot}");
        }

        string[] architectureRoots =
        {
            "aarch64-linux-ohos", "x86_64-linux-ohos", "arm-linux-ohos", "i686-linux-ohos",
            "riscv64-linux-ohos", "loongarch64-linux-ohos"
        };
        IEnumerable<string> files = Directory.EnumerateFiles(hashRoot, "*", SearchOption.AllDirectories)
            .Where(path => !headersOnly ||
                architectureRoots.Contains(
                    Path.GetRelativePath(hashRoot, path).Replace('\\', '/').Split('/')[0],
                    StringComparer.OrdinalIgnoreCase) is false &&
                Path.GetExtension(path) is ".h" or ".hpp" or ".inc")
            .OrderBy(path => Path.GetRelativePath(hashRoot, path).Replace('\\', '/'), StringComparer.Ordinal);

        using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        foreach (string file in files)
        {
            string relative = Path.GetRelativePath(hashRoot, file).Replace('\\', '/');
            string fileHash = ComputeFileSha256(file).ToLowerInvariant();
            string record = headersOnly
                ? $"{relative}\0{new FileInfo(file).Length}\0{fileHash}\n"
                : $"{relative}\0{fileHash}\n";
            AppendUtf8(hash, record);
        }

        return Convert.ToHexString(hash.GetHashAndReset());
    }

    private static void AppendUtf8(IncrementalHash hash, string value) =>
        hash.AppendData(Encoding.UTF8.GetBytes(value));
}
