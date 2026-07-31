using System.Text.Json;
using System.Text.Json.Serialization;
using OpenHarmony.Ndk.Bindings.Generator.Inventory;

namespace OpenHarmony.Ndk.Bindings.Generator.Diff;

public sealed record HeaderChange(
    [property: JsonPropertyName("path")] string Path,
    [property: JsonPropertyName("kind")] string Kind,
    [property: JsonPropertyName("beforeSha256")] string? BeforeSha256,
    [property: JsonPropertyName("afterSha256")] string? AfterSha256);

public sealed record ApiDiffDocument(
    [property: JsonPropertyName("schemaVersion")] int SchemaVersion,
    [property: JsonPropertyName("beforeApi")] int BeforeApi,
    [property: JsonPropertyName("afterApi")] int AfterApi,
    [property: JsonPropertyName("added")] IReadOnlyList<HeaderChange> Added,
    [property: JsonPropertyName("removed")] IReadOnlyList<HeaderChange> Removed,
    [property: JsonPropertyName("changed")] IReadOnlyList<HeaderChange> Changed);

public static class ApiDiff
{
    public static ApiDiffDocument Compare(HeaderInventoryDocument before, HeaderInventoryDocument after)
    {
        Dictionary<string, HeaderRecord> oldHeaders = before.Headers.ToDictionary(header => header.Path, StringComparer.Ordinal);
        Dictionary<string, HeaderRecord> newHeaders = after.Headers.ToDictionary(header => header.Path, StringComparer.Ordinal);

        HeaderChange[] added = newHeaders.Values
            .Where(header => !oldHeaders.ContainsKey(header.Path))
            .OrderBy(header => header.Path, StringComparer.Ordinal)
            .Select(header => new HeaderChange(header.Path, "added", null, header.Sha256))
            .ToArray();
        HeaderChange[] removed = oldHeaders.Values
            .Where(header => !newHeaders.ContainsKey(header.Path))
            .OrderBy(header => header.Path, StringComparer.Ordinal)
            .Select(header => new HeaderChange(header.Path, "removed", header.Sha256, null))
            .ToArray();
        HeaderChange[] changed = newHeaders.Values
            .Where(header => oldHeaders.TryGetValue(header.Path, out HeaderRecord? old) && !string.Equals(old.Sha256, header.Sha256, StringComparison.Ordinal))
            .OrderBy(header => header.Path, StringComparer.Ordinal)
            .Select(header => new HeaderChange(header.Path, "changed", oldHeaders[header.Path].Sha256, header.Sha256))
            .ToArray();

        return new ApiDiffDocument(1, before.ApiLevel, after.ApiLevel, added, removed, changed);
    }

    public static void Write(ApiDiffDocument document, string outputPath)
    {
        string fullPath = Path.GetFullPath(outputPath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true }) + Environment.NewLine);
    }
}
