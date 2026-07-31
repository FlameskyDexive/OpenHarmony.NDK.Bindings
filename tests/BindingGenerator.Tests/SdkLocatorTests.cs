using System.Security.Cryptography;
using OpenHarmony.Ndk.Bindings.Generator.Sdk;

namespace OpenHarmony.Ndk.Bindings.Generator.Tests;

public sealed class SdkLocatorTests
{
    [Theory]
    [InlineData(15, "15", "5.0.3.135", "Release")]
    [InlineData(18, "18", "5.1.0.107", "Release")]
    [InlineData(20, "20", "6.0.0.47", "Release")]
    [InlineData(23, "23", "6.1.0.32", "Release")]
    [InlineData(26, "26.0.0", "26.0.0.25", "Beta")]
    public void Resolve_reads_expected_native_sdk(
        int apiLevel,
        string folderName,
        string packageVersion,
        string releaseType)
    {
        using TemporarySdkRoot fixture = TemporarySdkRoot.Create(
            apiLevel,
            folderName,
            packageVersion,
            releaseType);

        SdkManifest manifest = fixture.CreateManifest("5.0");

        NativeSdk sdk = SdkLocator.Resolve(fixture.RootPath, manifest);

        Assert.Equal(folderName, sdk.Manifest.FolderName);
        Assert.Equal(packageVersion, sdk.Manifest.PackageVersion);
        Assert.Equal(releaseType, sdk.Manifest.ReleaseType);
        Assert.Equal(fixture.ClangPath, sdk.ClangPath);
        Assert.Equal(fixture.SysrootPath, sdk.SysrootPath);
        Assert.Equal(fixture.ToolchainFile, sdk.ToolchainFile);
    }

    [Fact]
    public void Resolve_rejects_a_package_version_mismatch()
    {
        using TemporarySdkRoot fixture = TemporarySdkRoot.Create(
            15,
            "15",
            "5.0.3.135",
            "Release");

        SdkManifest manifest = fixture.CreateManifest("5.0") with
        {
            PackageVersion = "unexpected"
        };

        InvalidDataException exception = Assert.Throws<InvalidDataException>(
            () => SdkLocator.Resolve(fixture.RootPath, manifest));

        Assert.Contains("package version", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("5.0.3.135", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Resolve_rejects_a_package_manifest_hash_mismatch()
    {
        using TemporarySdkRoot fixture = TemporarySdkRoot.Create(
            26,
            "26.0.0",
            "26.0.0.25",
            "Beta");

        SdkManifest manifest = fixture.CreateManifest("7") with
        {
            PackageManifestSha256 = new string('0', 64)
        };

        InvalidDataException exception = Assert.Throws<InvalidDataException>(
            () => SdkLocator.Resolve(fixture.RootPath, manifest));

        Assert.Contains("SHA256", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class TemporarySdkRoot : IDisposable
    {
        private TemporarySdkRoot(
            string rootPath,
            string packageManifestPath,
            string clangPath,
            string sysrootPath,
            string toolchainFile,
            int apiLevel,
            string folderName,
            string packageVersion,
            string releaseType)
        {
            RootPath = rootPath;
            PackageManifestPath = packageManifestPath;
            ClangPath = clangPath;
            SysrootPath = sysrootPath;
            ToolchainFile = toolchainFile;
            ApiLevel = apiLevel;
            FolderName = folderName;
            PackageVersion = packageVersion;
            ReleaseType = releaseType;
        }

        public string RootPath { get; }
        public string PackageManifestPath { get; }
        public string ClangPath { get; }
        public string SysrootPath { get; }
        public string ToolchainFile { get; }
        public int ApiLevel { get; }
        public string FolderName { get; }
        public string PackageVersion { get; }
        public string ReleaseType { get; }

        public static TemporarySdkRoot Create(
            int apiLevel,
            string folderName,
            string packageVersion,
            string releaseType)
        {
            string rootPath = Path.Combine(
                Path.GetTempPath(),
                $"openharmony-sdk-test-{Guid.NewGuid():N}");
            string nativePath = Path.Combine(rootPath, folderName, "native");
            string packageManifestPath = Path.Combine(nativePath, "oh-uni-package.json");
            string clangPath = Path.Combine(nativePath, "llvm", "bin", "clang.exe");
            string sysrootPath = Path.Combine(nativePath, "sysroot");
            string toolchainFile = Path.Combine(nativePath, "build", "cmake", "ohos.toolchain.cmake");

            Directory.CreateDirectory(Path.GetDirectoryName(clangPath)!);
            Directory.CreateDirectory(sysrootPath);
            Directory.CreateDirectory(Path.GetDirectoryName(toolchainFile)!);
            File.WriteAllText(clangPath, string.Empty);
            File.WriteAllText(toolchainFile, string.Empty);
            File.WriteAllText(
                packageManifestPath,
                $$"""
                {
                  "apiVersion": "{{apiLevel}}",
                  "displayName": "Native",
                  "meta": { "metaVersion": "3.0.0" },
                  "path": "native",
                  "releaseType": "{{releaseType}}",
                  "version": "{{packageVersion}}"
                }
                """);

            return new TemporarySdkRoot(
                rootPath,
                packageManifestPath,
                clangPath,
                sysrootPath,
                toolchainFile,
                apiLevel,
                folderName,
                packageVersion,
                releaseType);
        }

        public SdkManifest CreateManifest(string harmonyVersion)
        {
            string sha256 = Convert.ToHexString(
                SHA256.HashData(File.ReadAllBytes(PackageManifestPath)));

            return new SdkManifest(
                ApiLevel,
                harmonyVersion,
                FolderName,
                PackageVersion,
                ReleaseType,
                sha256);
        }

        public void Dispose()
        {
            Directory.Delete(RootPath, recursive: true);
        }
    }
}
