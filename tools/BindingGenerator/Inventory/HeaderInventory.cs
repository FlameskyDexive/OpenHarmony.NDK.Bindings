using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenHarmony.Ndk.Bindings.Generator.Inventory;

public sealed record HeaderRecord(
    [property: JsonPropertyName("path")] string Path,
    [property: JsonPropertyName("size")] long Size,
    [property: JsonPropertyName("sha256")] string Sha256);

public sealed record HeaderInventoryDocument(
    [property: JsonPropertyName("schemaVersion")] int SchemaVersion,
    [property: JsonPropertyName("apiLevel")] int ApiLevel,
    [property: JsonPropertyName("sysroot")] string Sysroot,
    [property: JsonPropertyName("headers")] IReadOnlyList<HeaderRecord> Headers);

public static class HeaderInventory
{
    private static readonly HashSet<string> ArchitectureRoots = new(StringComparer.OrdinalIgnoreCase)
    {
        "aarch64-linux-ohos", "x86_64-linux-ohos", "arm-linux-ohos", "i686-linux-ohos",
        "riscv64-linux-ohos", "loongarch64-linux-ohos"
    };

    public static HeaderInventoryDocument Scan(string sysroot, int apiLevel)
    {
        string root = Path.GetFullPath(sysroot);
        string includeRoot = Path.Combine(root, "usr", "include");
        if (!Directory.Exists(includeRoot))
        {
            throw new DirectoryNotFoundException($"SDK include root was not found: {includeRoot}");
        }

        HeaderRecord[] headers = Directory.EnumerateFiles(includeRoot, "*", SearchOption.AllDirectories)
            .Where(path => IsHeader(path, includeRoot))
            .Select(path => CreateRecord(path, includeRoot))
            .OrderBy(header => header.Path, StringComparer.Ordinal)
            .ToArray();

        return new HeaderInventoryDocument(1, apiLevel, "native/sysroot", headers);
    }

    public static void Write(HeaderInventoryDocument document, string outputPath)
    {
        ArgumentNullException.ThrowIfNull(document);
        string fullPath = Path.GetFullPath(outputPath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        JsonSerializerOptions options = new() { WriteIndented = true };
        File.WriteAllText(fullPath, JsonSerializer.Serialize(document, options) + Environment.NewLine);
    }

    public static HeaderInventoryDocument Read(string path)
    {
        HeaderInventoryDocument document = JsonSerializer.Deserialize<HeaderInventoryDocument>(
            File.ReadAllText(path),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidDataException($"Header inventory is empty: {path}");

        if (document.SchemaVersion != 1 || document.ApiLevel <= 0 || document.Headers.Any(header => string.IsNullOrWhiteSpace(header.Path)))
        {
            throw new InvalidDataException($"Header inventory contains invalid fields: {path}");
        }

        return document with
        {
            Headers = document.Headers.OrderBy(header => header.Path, StringComparer.Ordinal).ToArray()
        };
    }

    private static bool IsHeader(string path, string includeRoot)
    {
        string relative = Path.GetRelativePath(includeRoot, path).Replace('\\', '/');
        string firstSegment = relative.Split('/')[0];
        if (ArchitectureRoots.Contains(firstSegment)) return false;
        return Path.GetExtension(path).Equals(".h", StringComparison.OrdinalIgnoreCase) ||
               Path.GetExtension(path).Equals(".hpp", StringComparison.OrdinalIgnoreCase) ||
               Path.GetExtension(path).Equals(".inc", StringComparison.OrdinalIgnoreCase);
    }

    private static HeaderRecord CreateRecord(string path, string includeRoot)
    {
        using FileStream stream = File.OpenRead(path);
        return new HeaderRecord(
            Path.GetRelativePath(includeRoot, path).Replace('\\', '/'),
            stream.Length,
            Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant());
    }
}
