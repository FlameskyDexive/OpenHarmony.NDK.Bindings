using OpenHarmony.Ndk.Bindings.Generator.Sdk;
using OpenHarmony.Ndk.Bindings.Generator.Diff;
using OpenHarmony.Ndk.Bindings.Generator.Inventory;
using OpenHarmony.Ndk.Bindings.Generator.Coverage;

return ProgramEntry.Run(args);

internal static class ProgramEntry
{
    private const string Usage =
        "Usage: BindingGenerator verify-sdk --sdk-root <path> --apis <13..24,26> [--manifest-directory <path>] | " +
        "inventory --sdk-root <path> --api <level> --output <path> [--manifest-directory <path>] | " +
        "diff --before <path> --after <path> --output <path> | " +
        "coverage --sysroot <path> --config <path> --output <path>";

    public static int Run(string[] args)
    {
        try
        {
            if (args.Length == 0)
            {
                throw new ArgumentException(Usage);
            }

            if (string.Equals(args[0], "inventory", StringComparison.Ordinal))
            {
                return RunInventory(ParseOptions(args[1..]));
            }

            if (string.Equals(args[0], "diff", StringComparison.Ordinal))
            {
                return RunDiff(ParseOptions(args[1..]));
            }

            if (string.Equals(args[0], "coverage", StringComparison.Ordinal))
            {
                return RunCoverage(ParseOptions(args[1..]));
            }

            if (!string.Equals(args[0], "verify-sdk", StringComparison.Ordinal))
            {
                throw new ArgumentException(Usage);
            }

            Dictionary<string, string> options = ParseOptions(args[1..]);
            string sdkRoot = RequireOption(options, "--sdk-root");
            string manifestDirectory = options.GetValueOrDefault(
                "--manifest-directory",
                Path.Combine(Environment.CurrentDirectory, "sdk-manifests"));
            int[] apiLevels = ParseApiLevels(RequireOption(options, "--apis"));
            foreach (int apiLevel in apiLevels)
            {
                ApiAvailability.RequireSupported(apiLevel);
            }
            IReadOnlyDictionary<int, SdkManifest> manifests =
                SdkManifest.LoadDirectory(manifestDirectory);

            Console.WriteLine("API\tHarmonyOS\tPackage\tChannel\tSDK folder\tClang");
            int skipped = 0;
            foreach (int apiLevel in apiLevels)
            {
                if (!manifests.TryGetValue(apiLevel, out SdkManifest? manifest))
                {
                    throw new InvalidDataException(
                        $"No checked-in SDK manifest exists for API {apiLevel}: {manifestDirectory}");
                }

                if (!manifest.NativeSdkAvailable)
                {
                    Console.WriteLine($"SKIP API {apiLevel}: {manifest.UnavailableReason}");
                    skipped++;
                    continue;
                }

                NativeSdk sdk = SdkLocator.Resolve(sdkRoot, manifest);
                Console.WriteLine(
                    $"{apiLevel}\t{manifest.HarmonyVersion}\t{manifest.PackageVersion}\t" +
                    $"{manifest.ReleaseType}\t{Path.GetDirectoryName(Path.GetDirectoryName(sdk.PackageManifestPath))}\t" +
                    $"{sdk.ClangPath}");
            }

            if (skipped > 0)
            {
                Console.WriteLine($"Skipped {skipped} API level(s) without an installable Native SDK.");
            }

            return 0;
        }
        catch (Exception exception) when (
            exception is ArgumentException or IOException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine(exception.Message);
            return 1;
        }
    }

    private static int RunInventory(IReadOnlyDictionary<string, string> options)
    {
        string sdkRoot = RequireOption(options, "--sdk-root");
        string output = RequireOption(options, "--output");
        int api = ParseApiLevels(RequireOption(options, "--api")).Single();
        ApiAvailability.RequireSupported(api);
        string manifestDirectory = options.GetValueOrDefault("--manifest-directory", Path.Combine(Environment.CurrentDirectory, "sdk-manifests"));
        IReadOnlyDictionary<int, SdkManifest> manifests = SdkManifest.LoadDirectory(manifestDirectory);
        if (!manifests.TryGetValue(api, out SdkManifest? manifest)) throw new InvalidDataException($"No checked-in SDK manifest exists for API {api}: {manifestDirectory}");
        if (!manifest.NativeSdkAvailable) throw new InvalidDataException($"HarmonyOS API {api} has no installable Native SDK and is not processed for compilation. {manifest.UnavailableReason}");
        NativeSdk sdk = SdkLocator.Resolve(sdkRoot, manifest);
        HeaderInventory.Write(HeaderInventory.Scan(sdk.SysrootPath, api), output);
        return 0;
    }

    private static int RunDiff(IReadOnlyDictionary<string, string> options)
    {
        ApiDiffDocument diff = ApiDiff.Compare(
            HeaderInventory.Read(RequireOption(options, "--before")),
            HeaderInventory.Read(RequireOption(options, "--after")));
        ApiDiff.Write(diff, RequireOption(options, "--output"));
        return 0;
    }

    private static int RunCoverage(IReadOnlyDictionary<string, string> options)
    {
        PublicKitAllowlist allowlist = PublicKitCoverage.Load(RequireOption(options, "--config"));
        PublicKitCoverageReport report = PublicKitCoverage.Scan(RequireOption(options, "--sysroot"), allowlist);
        PublicKitCoverage.Write(report, RequireOption(options, "--output"));
        if (report.MissingHeaderRoots.Count != 0 || report.UnexplainedHeaderRoots.Count != 0)
            throw new InvalidDataException($"API {report.ApiLevel} public-kit coverage is incomplete: missing={report.MissingHeaderRoots.Count}, unexplained={report.UnexplainedHeaderRoots.Count}.");
        return 0;
    }

    private static Dictionary<string, string> ParseOptions(string[] args)
    {
        if (args.Length % 2 != 0)
        {
            throw new ArgumentException(Usage);
        }

        Dictionary<string, string> options = new(StringComparer.Ordinal);
        for (int index = 0; index < args.Length; index += 2)
        {
            if (!args[index].StartsWith("--", StringComparison.Ordinal) ||
                !options.TryAdd(args[index], args[index + 1]))
            {
                throw new ArgumentException(Usage);
            }
        }

        return options;
    }

    private static string RequireOption(IReadOnlyDictionary<string, string> options, string name)
    {
        if (!options.TryGetValue(name, out string? value) || string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Missing required option {name}. {Usage}");
        }

        return value;
    }

    private static int[] ParseApiLevels(string value)
    {
        int[] apiLevels = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(part => int.TryParse(part, out int apiLevel)
                ? apiLevel
                : throw new ArgumentException($"Invalid API level '{part}'."))
            .Distinct()
            .Order()
            .ToArray();

        return apiLevels.Length > 0
            ? apiLevels
            : throw new ArgumentException("At least one API level is required.");
    }
}
