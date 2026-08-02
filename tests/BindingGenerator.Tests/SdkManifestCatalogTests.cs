using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using OpenHarmony.Ndk.Bindings.Generator.Sdk;

namespace OpenHarmony.Ndk.Bindings.Generator.Tests;

public sealed class SdkManifestCatalogTests
{
    private const string Api25Error =
        "API 25 is intentionally unsupported because no compatible SDK/image is available.";

    private static readonly ManifestFact[] Facts =
    {
        new(13, "5.0.1", "13", "5.0.1.111", "Release", "435560BC7D74005A306E8EA04110FC19E5392556D6B10493CC9DD93F7E7F1853", "4239E6EBF2C135EC7B803E4AD23326C4BFFC3E4B5CBB4BA2F07135D88B8E8F44", "FB20370A9B3E77654DC938CBBC31F7C78432C9E01F8A3F2CAAA8EABCA0353797", "5DFC8072E4CC5B93330814F0D4C21EA08A41D9A99A9593F3BA21A500779D9C3E"),
        new(14, "5.0.2", "14", "5.0.2.123", "Release", "0BC0ED1C01345BBA9B453E93F5FAF4B4F55F9D17850161D58742A734E324956C", "C5EFC7788ECFC24D095ABD0AB26475097C926EA1C20D74F19F9CBEEDCA742555", "FB20370A9B3E77654DC938CBBC31F7C78432C9E01F8A3F2CAAA8EABCA0353797", "77C6504C73FF24163F4CBBCB655CCAFA7C755CC439344B767C7874BC917F1070"),
        new(15, "5.0.3", "15", "5.0.3.135", "Release", "0E0DB0B79DF8A26E18B7BD17FCCED06242D66C8315822F4037A357BC211CECC8", "EBB273A191F40D33CE7567858DA8D931CAB2BBDD8DD9CE34771B916DE7FFB2AC", "FB20370A9B3E77654DC938CBBC31F7C78432C9E01F8A3F2CAAA8EABCA0353797", "F2FEA481F376FDFC5A0012BCDD87D0AFD1CF0FF988910AC17615FCA4C22E3B5B"),
        new(18, "5.1.0", "18", "5.1.0.107", "Release", "16CAC01A0F3829A89353E7117E823DA2A5D4F3BE1E6C3250024409F39F85F987", "E098D4272091715BC7AA47DDC9785BF18C10651F33FF354BEBB2D7178C1BAE24", "37E82984965896B273DBBA13F7233C1352DCA7CAAE886AC0422219D37A3AA75A", "9C138A4D2C280FA12A82B3980C4275F626D15389C41628CB135050D6BDF5CF6E"),
        new(20, "6.0.0", "20", "6.0.0.47", "Release", "E7D4E0F7970EC5D8430F2935DC22B31EFFE06D9E8C725A16FF0706F8C60EC22C", "595202754A24F7EE977DFC010006D0290A9524C85C343045E9F42DF5DC0BE41F", "37E82984965896B273DBBA13F7233C1352DCA7CAAE886AC0422219D37A3AA75A", "9F60D3ED00C46966EFDF927D0079C5F03849D2F40631FA9EDF5F6A6808EFB801"),
        new(23, "6.1.0", "23", "6.1.0.32", "Release", "475FA68431FCEA117512CCEA5BF303D0FA6BD9A2C2E4C6DCBE6567FD1CED3B44", "17157540D875F5595DF36CBD6AF584480AA9BDDEB2E7FE549E6C40316BA0FEA5", "0CE9943DF04C192725B41CD70DBB259EDD21FA9FA8D78DEEE0D6254F4D6FEB18", "E648CAFF2F36A4F0E14A785E63B8371B14EA45E4CF723E356BB272EDD2594010"),
        new(26, "26.0.0", "26.0.0", "26.0.0.25", "Beta", "EE77347D990B6F99C07777993FB372E5FB55AA08E76A4C82E3253F8F9E094B16", "4AFBACD62E4D068685CD88F6CEFF7F01D9E7004E827464F998049C7D50E2A8B0", "2BE06202DBD5138499BD60146C28A01DAE071B7E1DD0E8C5543897F1D722350B", "69BF7F9E17A13CD8DB334A5DB03D6683EA7312B82DA3EA2687687508B4261623")
    };

    private static readonly int[] NativeUnavailableApis = { 16, 17, 19, 21, 22, 24 };

    [Fact]
    public void Checked_in_catalog_contains_exact_native_sdk_facts()
    {
        string manifestDirectory = Path.Combine(FindRepoRoot(), "sdk-manifests");
        string[] files = Directory.GetFiles(manifestDirectory, "harmonyos-api*.json")
            .Order(StringComparer.Ordinal)
            .ToArray();

        int[] allApis = Enumerable.Range(13, 12).Append(26).ToArray();
        Assert.Equal(
            allApis.Select(api => $"harmonyos-api{api}.json").ToArray(),
            files.Select(Path.GetFileName).ToArray());
        for (int index = 0; index < Facts.Length; index++)
        {
            string path = files.Single(file => file.EndsWith($"api{Facts[index].ApiLevel}.json", StringComparison.Ordinal));
            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path));
            JsonElement root = document.RootElement;
            ManifestFact expected = Facts[index];
            Assert.Equal(expected.ApiLevel, root.GetProperty("apiLevel").GetInt32());
            Assert.Equal(expected.HarmonyVersion, root.GetProperty("harmonyVersion").GetString());
            Assert.Equal(expected.FolderName, root.GetProperty("folderName").GetString());
            Assert.Equal(expected.PackageVersion, root.GetProperty("packageVersion").GetString());
            Assert.Equal(expected.ReleaseType, root.GetProperty("releaseType").GetString());
            Assert.Equal(expected.PackageManifestSha256, root.GetProperty("packageManifestSha256").GetString());
            Assert.Equal(expected.SysrootSha256, root.GetProperty("sysrootSha256").GetString());
            Assert.Equal(expected.ToolchainFileSha256, root.GetProperty("toolchainFileSha256").GetString());
            Assert.Equal(expected.HeaderInventorySha256, root.GetProperty("headerInventorySha256").GetString());
        }

        foreach (int api in NativeUnavailableApis)
        {
            string path = files.Single(file => file.EndsWith($"api{api}.json", StringComparison.Ordinal));
            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path));
            Assert.False(document.RootElement.GetProperty("nativeSdkAvailable").GetBoolean());
            Assert.Contains("No independent Native SDK package is installable", document.RootElement.GetProperty("unavailableReason").GetString(), StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Checked_in_catalog_loads_unavailable_manifests_without_native_package_fields()
    {
        IReadOnlyDictionary<int, SdkManifest> manifests = SdkManifest.LoadDirectory(
            Path.Combine(FindRepoRoot(), "sdk-manifests"));

        foreach (int api in NativeUnavailableApis)
        {
            SdkManifest manifest = manifests[api];
            Assert.False(manifest.NativeSdkAvailable);
            Assert.Null(manifest.ReleaseType);
            Assert.Contains("No independent Native SDK package is installable", manifest.UnavailableReason, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Locator_rejects_a_sysroot_fingerprint_mismatch()
    {
        using TemporaryManifestSdk fixture = TemporaryManifestSdk.Create();
        IReadOnlyDictionary<int, SdkManifest> manifests = SdkManifest.LoadDirectory(fixture.ManifestDirectory);
        File.WriteAllText(Path.Combine(fixture.SysrootPath, "usr", "include", "changed.h"), "changed");

        InvalidDataException exception = Assert.Throws<InvalidDataException>(
            () => SdkLocator.Resolve(fixture.SdkRoot, manifests[13]));

        Assert.Contains("sysroot SHA256", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Sdk_fingerprints_are_deterministic()
    {
        using TemporaryManifestSdk fixture = TemporaryManifestSdk.Create();

        Assert.Equal(
            SdkFingerprint.ComputeSysrootSha256(fixture.SysrootPath),
            SdkFingerprint.ComputeSysrootSha256(fixture.SysrootPath));
        Assert.Equal(
            SdkFingerprint.ComputeHeaderInventorySha256(fixture.SysrootPath, 13),
            SdkFingerprint.ComputeHeaderInventorySha256(fixture.SysrootPath, 13));
    }

    [Fact]
    public void Cli_rejects_api25_with_the_exact_message()
    {
        string repoRoot = FindRepoRoot();
        ProcessResult result = RunProcess(
            "dotnet",
            $"\"{typeof(SdkManifest).Assembly.Location}\" verify-sdk --sdk-root \"missing\" --apis 25 --manifest-directory \"{Path.Combine(repoRoot, "sdk-manifests")}\"",
            repoRoot);

        Assert.Equal(1, result.ExitCode);
        Assert.Equal(Api25Error, result.StandardError.Trim());
    }

    [Fact]
    public void Abi_matrix_explicitly_skips_native_unavailable_apis()
    {
        string repoRoot = FindRepoRoot();
        string tempRoot = Path.Combine(Path.GetTempPath(), "openharmony-abi-skip-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);
        try
        {
            ProcessResult result = RunProcess(
                "pwsh",
                $"-NoProfile -Command \"& '{Path.Combine(repoRoot, "tools", "run-abi-matrix.ps1")}' -SdkRoot '{tempRoot}' -Apis @(16,17,19,21,22,24) -OutputRoot '{Path.Combine(tempRoot, "output")}'\"",
                repoRoot);

            Assert.Equal(0, result.ExitCode);
            foreach (int api in new[] { 16, 17, 19, 21, 22, 24 })
                Assert.Contains($"SKIP API {api}:", result.StandardOutput, StringComparison.Ordinal);
            Assert.Contains("skipped 6 API level(s)", result.StandardOutput, StringComparison.Ordinal);
        }
        finally
        {
            Directory.Delete(tempRoot, recursive: true);
        }
    }

    [Fact]
    public void Abi_matrix_default_targets_the_complete_supported_api_set()
    {
        string script = File.ReadAllText(Path.Combine(FindRepoRoot(), "tools", "run-abi-matrix.ps1"));

        Assert.Contains(
            "$Apis = @(13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 26)",
            script,
            StringComparison.Ordinal);
    }

    private static ProcessResult RunProcess(string fileName, string arguments, string workingDirectory)
    {
        using Process process = Process.Start(new ProcessStartInfo(fileName, arguments)
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        })!;
        string stdout = process.StandardOutput.ReadToEnd();
        string stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();
        return new ProcessResult(process.ExitCode, stdout, stderr);
    }

    private static string FindRepoRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "OpenHarmony.NDK.Bindings.sln")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root was not found.");
    }

    private sealed class TemporaryManifestSdk : IDisposable
    {
        private TemporaryManifestSdk(string root, string sdkRoot, string manifestDirectory, string sysrootPath)
        {
            Root = root; SdkRoot = sdkRoot; ManifestDirectory = manifestDirectory; SysrootPath = sysrootPath;
        }
        public string Root { get; }
        public string SdkRoot { get; }
        public string ManifestDirectory { get; }
        public string SysrootPath { get; }

        public static TemporaryManifestSdk Create()
        {
            string root = Path.Combine(Path.GetTempPath(), "openharmony-manifest-test-" + Guid.NewGuid().ToString("N"));
            string sdkRoot = Path.Combine(root, "sdk");
            string native = Path.Combine(sdkRoot, "13", "native");
            string manifestDirectory = Path.Combine(root, "manifests");
            string sysroot = Path.Combine(native, "sysroot");
            string include = Path.Combine(sysroot, "usr", "include");
            string toolchain = Path.Combine(native, "build", "cmake", "ohos.toolchain.cmake");
            string package = Path.Combine(native, "oh-uni-package.json");
            string clang = Path.Combine(native, "llvm", "bin", "clang.exe");
            Directory.CreateDirectory(include);
            Directory.CreateDirectory(Path.GetDirectoryName(toolchain)!);
            Directory.CreateDirectory(Path.GetDirectoryName(clang)!);
            Directory.CreateDirectory(manifestDirectory);
            File.WriteAllText(Path.Combine(include, "sample.h"), "sample");
            File.WriteAllText(toolchain, "toolchain");
            File.WriteAllText(clang, string.Empty);
            File.WriteAllText(package, "{\"apiVersion\":\"13\",\"version\":\"test\",\"releaseType\":\"Release\"}");
            string packageHash = FileHash(package);
            string toolchainHash = FileHash(toolchain);
            string sysrootHash = TreeHash(sysroot, headersOnly: false);
            string headerHash = TreeHash(sysroot, headersOnly: true);
            File.WriteAllText(Path.Combine(manifestDirectory, "harmonyos-api13.json"), $$"""
                {
                  "apiLevel": 13,
                  "harmonyVersion": "5.0.1",
                  "folderName": "13",
                  "packageVersion": "test",
                  "releaseType": "Release",
                  "packageManifestSha256": "{{packageHash}}",
                  "sysrootSha256": "{{sysrootHash}}",
                  "toolchainFileSha256": "{{toolchainHash}}",
                  "headerInventorySha256": "{{headerHash}}"
                }
                """);
            return new TemporaryManifestSdk(root, sdkRoot, manifestDirectory, sysroot);
        }

        public void Dispose() => Directory.Delete(Root, recursive: true);
    }

    private static string FileHash(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));

    private static string TreeHash(string sysroot, bool headersOnly)
    {
        string root = headersOnly ? Path.Combine(sysroot, "usr", "include") : sysroot;
        using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        foreach (string path in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories).Order(StringComparer.Ordinal))
        {
            string relative = Path.GetRelativePath(root, path).Replace('\\', '/');
            string fileHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
            string record = headersOnly
                ? $"{relative}\0{new FileInfo(path).Length}\0{fileHash}\n"
                : $"{relative}\0{fileHash}\n";
            hash.AppendData(Encoding.UTF8.GetBytes(record));
        }
        return Convert.ToHexString(hash.GetHashAndReset());
    }

    private sealed record ManifestFact(int ApiLevel, string HarmonyVersion, string FolderName, string PackageVersion, string ReleaseType, string PackageManifestSha256, string SysrootSha256, string ToolchainFileSha256, string HeaderInventorySha256);
    private sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError);
}
