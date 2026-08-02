using System.Text.Json;

namespace OpenHarmony.Ndk.Bindings.Generator.Sdk;

public sealed record SdkManifest(
    int ApiLevel,
    string HarmonyVersion,
    string? FolderName,
    string? PackageVersion,
    string? ReleaseType,
    string PackageManifestSha256,
    bool NativeSdkAvailable = true,
    string? UnavailableReason = null,
    string? SysrootSha256 = null,
    string? ToolchainFileSha256 = null,
    string? HeaderInventorySha256 = null)
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static IReadOnlyDictionary<int, SdkManifest> LoadDirectory(string manifestDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(manifestDirectory);

        string fullPath = Path.GetFullPath(manifestDirectory);
        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException($"SDK manifest directory was not found: {fullPath}");
        }

        Dictionary<int, SdkManifest> manifests = new();
        foreach (string path in Directory.EnumerateFiles(fullPath, "harmonyos-api*.json")
                     .OrderBy(path => path, StringComparer.Ordinal))
        {
            SdkManifest manifest = JsonSerializer.Deserialize<SdkManifest>(
                File.ReadAllText(path),
                SerializerOptions)
                ?? throw new InvalidDataException($"SDK manifest is empty: {path}");

            Validate(manifest, path);
            if (!manifests.TryAdd(manifest.ApiLevel, manifest))
            {
                throw new InvalidDataException(
                    $"Duplicate SDK manifest for API {manifest.ApiLevel}: {path}");
            }
        }

        if (manifests.Count == 0)
        {
            throw new InvalidDataException($"No HarmonyOS SDK manifests were found in: {fullPath}");
        }

        return manifests;
    }

    private static void Validate(SdkManifest manifest, string path)
    {
        if (manifest.ApiLevel <= 0 ||
            string.IsNullOrWhiteSpace(manifest.HarmonyVersion))
        {
            throw new InvalidDataException($"SDK manifest contains invalid fields: {path}");
        }

        if (!manifest.NativeSdkAvailable)
        {
            if (string.IsNullOrWhiteSpace(manifest.UnavailableReason))
            {
                throw new InvalidDataException($"Unavailable SDK manifest must explain the unavailable API: {path}");
            }
            return;
        }

        if (string.IsNullOrWhiteSpace(manifest.FolderName) ||
            string.IsNullOrWhiteSpace(manifest.PackageVersion) ||
            !IsSha256(manifest.PackageManifestSha256) ||
            !IsSha256(manifest.SysrootSha256) ||
            !IsSha256(manifest.ToolchainFileSha256) ||
            !IsSha256(manifest.HeaderInventorySha256))
        {
            throw new InvalidDataException($"Native SDK manifest contains invalid fields: {path}");
        }
    }

    private static bool IsSha256(string? value) =>
        value?.Length == 64 && value.All(Uri.IsHexDigit);
}
