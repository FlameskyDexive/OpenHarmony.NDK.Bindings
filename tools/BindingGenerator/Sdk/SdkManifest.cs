using System.Text.Json;

namespace OpenHarmony.Ndk.Bindings.Generator.Sdk;

public sealed record SdkManifest(
    int ApiLevel,
    string HarmonyVersion,
    string FolderName,
    string PackageVersion,
    string ReleaseType,
    string PackageManifestSha256)
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
            string.IsNullOrWhiteSpace(manifest.HarmonyVersion) ||
            string.IsNullOrWhiteSpace(manifest.FolderName) ||
            string.IsNullOrWhiteSpace(manifest.PackageVersion) ||
            string.IsNullOrWhiteSpace(manifest.ReleaseType) ||
            manifest.PackageManifestSha256.Length != 64 ||
            !manifest.PackageManifestSha256.All(Uri.IsHexDigit))
        {
            throw new InvalidDataException($"SDK manifest contains invalid fields: {path}");
        }
    }
}
