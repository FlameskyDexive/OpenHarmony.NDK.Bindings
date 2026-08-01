using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenHarmony.Ndk.Bindings.Generator.Coverage;

public sealed record ExcludedHeaderRoot(
    [property: JsonPropertyName("path")] string Path,
    [property: JsonPropertyName("reason")] string Reason);

public sealed record PublicKitAllowlist(
    [property: JsonPropertyName("apiLevel")] int ApiLevel,
    [property: JsonPropertyName("headerRoots")] IReadOnlyList<string> HeaderRoots,
    [property: JsonPropertyName("excludedRoots")] IReadOnlyList<ExcludedHeaderRoot> ExcludedRoots);

public sealed record PublicKitCoverageReport(
    [property: JsonPropertyName("apiLevel")] int ApiLevel,
    [property: JsonPropertyName("discoveredHeaderRoots")] IReadOnlyList<string> DiscoveredHeaderRoots,
    [property: JsonPropertyName("coveredHeaderRoots")] IReadOnlyList<string> CoveredHeaderRoots,
    [property: JsonPropertyName("excludedHeaderRoots")] IReadOnlyList<ExcludedHeaderRoot> ExcludedHeaderRoots,
    [property: JsonPropertyName("missingHeaderRoots")] IReadOnlyList<string> MissingHeaderRoots,
    [property: JsonPropertyName("unexplainedHeaderRoots")] IReadOnlyList<string> UnexplainedHeaderRoots);

public static class PublicKitCoverage
{
    public static PublicKitAllowlist Load(string path) =>
        JsonSerializer.Deserialize<PublicKitAllowlist>(File.ReadAllText(path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        ?? throw new InvalidDataException($"Public kit allowlist is empty: {path}");

    public static PublicKitCoverageReport Scan(string sysroot, PublicKitAllowlist allowlist)
    {
        string includeRoot = Path.Combine(Path.GetFullPath(sysroot), "usr", "include");
        string[] discovered = Directory.EnumerateDirectories(includeRoot)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Cast<string>()
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        HashSet<string> covered = allowlist.HeaderRoots.ToHashSet(StringComparer.OrdinalIgnoreCase);
        HashSet<string> excluded = allowlist.ExcludedRoots.Select(item => item.Path).ToHashSet(StringComparer.OrdinalIgnoreCase);
        string[] missing = covered.Except(discovered, StringComparer.OrdinalIgnoreCase).OrderBy(value => value, StringComparer.OrdinalIgnoreCase).ToArray();
        string[] unexplained = discovered.Except(covered, StringComparer.OrdinalIgnoreCase).Except(excluded, StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase).ToArray();
        return new PublicKitCoverageReport(allowlist.ApiLevel, discovered, covered.OrderBy(value => value, StringComparer.OrdinalIgnoreCase).ToArray(),
            allowlist.ExcludedRoots.OrderBy(value => value.Path, StringComparer.OrdinalIgnoreCase).ToArray(), missing, unexplained);
    }

    public static void Write(PublicKitCoverageReport report, string outputPath)
    {
        string fullPath = Path.GetFullPath(outputPath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }) + Environment.NewLine);
    }
}
