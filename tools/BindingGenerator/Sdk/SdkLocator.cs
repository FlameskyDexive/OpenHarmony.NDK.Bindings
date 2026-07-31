using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenHarmony.Ndk.Bindings.Generator.Sdk;

public sealed record NativeSdk(
    SdkManifest Manifest,
    string RootPath,
    string ClangPath,
    string SysrootPath,
    string ToolchainFile,
    string PackageManifestPath);

public static class SdkLocator
{
    public static NativeSdk Resolve(string sdkRoot, SdkManifest expected)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sdkRoot);
        ArgumentNullException.ThrowIfNull(expected);

        string rootPath = Path.GetFullPath(sdkRoot);
        string nativePath = Path.Combine(rootPath, expected.FolderName, "native");
        string packageManifestPath = RequireFile(
            Path.Combine(nativePath, "oh-uni-package.json"),
            $"HarmonyOS API {expected.ApiLevel} native package manifest");

        NativePackageManifest package = JsonSerializer.Deserialize<NativePackageManifest>(
            File.ReadAllText(packageManifestPath),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidDataException(
                $"HarmonyOS API {expected.ApiLevel} native package manifest is empty: {packageManifestPath}");

        RequireEqual("API version", expected.ApiLevel.ToString(), package.ApiVersion, packageManifestPath);
        RequireEqual("package version", expected.PackageVersion, package.Version, packageManifestPath);
        RequireEqual("release type", expected.ReleaseType, package.ReleaseType, packageManifestPath);

        string actualSha256 = Convert.ToHexString(
            SHA256.HashData(File.ReadAllBytes(packageManifestPath)));
        RequireEqual(
            "package manifest SHA256",
            expected.PackageManifestSha256,
            actualSha256,
            packageManifestPath,
            StringComparison.OrdinalIgnoreCase);

        string clangPath = ResolveExecutable(Path.Combine(nativePath, "llvm", "bin"), "clang");
        string sysrootPath = RequireDirectory(
            Path.Combine(nativePath, "sysroot"),
            $"HarmonyOS API {expected.ApiLevel} sysroot");
        string toolchainFile = RequireFile(
            Path.Combine(nativePath, "build", "cmake", "ohos.toolchain.cmake"),
            $"HarmonyOS API {expected.ApiLevel} CMake toolchain");

        return new NativeSdk(
            expected,
            rootPath,
            clangPath,
            sysrootPath,
            toolchainFile,
            packageManifestPath);
    }

    private static string ResolveExecutable(string directory, string executableName)
    {
        foreach (string fileName in new[] { $"{executableName}.exe", executableName })
        {
            string path = Path.Combine(directory, fileName);
            if (File.Exists(path))
            {
                return Path.GetFullPath(path);
            }
        }

        throw new FileNotFoundException(
            $"HarmonyOS compiler was not found under: {Path.GetFullPath(directory)}");
    }

    private static string RequireFile(string path, string description)
    {
        string fullPath = Path.GetFullPath(path);
        return File.Exists(fullPath)
            ? fullPath
            : throw new FileNotFoundException($"{description} was not found: {fullPath}", fullPath);
    }

    private static string RequireDirectory(string path, string description)
    {
        string fullPath = Path.GetFullPath(path);
        return Directory.Exists(fullPath)
            ? fullPath
            : throw new DirectoryNotFoundException($"{description} was not found: {fullPath}");
    }

    private static void RequireEqual(
        string field,
        string expected,
        string? actual,
        string path,
        StringComparison comparison = StringComparison.Ordinal)
    {
        if (!string.Equals(expected, actual, comparison))
        {
            throw new InvalidDataException(
                $"HarmonyOS SDK {field} mismatch in {path}: expected '{expected}', found '{actual ?? "<missing>"}'.");
        }
    }

    private sealed record NativePackageManifest(
        [property: JsonPropertyName("apiVersion")] string? ApiVersion,
        [property: JsonPropertyName("version")] string? Version,
        [property: JsonPropertyName("releaseType")] string? ReleaseType);
}
