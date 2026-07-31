using OpenHarmony.Ndk.Bindings.Generator.Sdk;

return ProgramEntry.Run(args);

internal static class ProgramEntry
{
    private const string Usage =
        "Usage: BindingGenerator verify-sdk --sdk-root <path> --apis <15,18,20,23,26> " +
        "[--manifest-directory <path>]";

    public static int Run(string[] args)
    {
        try
        {
            if (args.Length == 0 || !string.Equals(args[0], "verify-sdk", StringComparison.Ordinal))
            {
                throw new ArgumentException(Usage);
            }

            Dictionary<string, string> options = ParseOptions(args[1..]);
            string sdkRoot = RequireOption(options, "--sdk-root");
            string manifestDirectory = options.GetValueOrDefault(
                "--manifest-directory",
                Path.Combine(Environment.CurrentDirectory, "sdk-manifests"));
            int[] apiLevels = ParseApiLevels(RequireOption(options, "--apis"));
            IReadOnlyDictionary<int, SdkManifest> manifests =
                SdkManifest.LoadDirectory(manifestDirectory);

            Console.WriteLine("API\tHarmonyOS\tPackage\tChannel\tSDK folder\tClang");
            foreach (int apiLevel in apiLevels)
            {
                if (!manifests.TryGetValue(apiLevel, out SdkManifest? manifest))
                {
                    throw new InvalidDataException(
                        $"No checked-in SDK manifest exists for API {apiLevel}: {manifestDirectory}");
                }

                NativeSdk sdk = SdkLocator.Resolve(sdkRoot, manifest);
                Console.WriteLine(
                    $"{apiLevel}\t{manifest.HarmonyVersion}\t{manifest.PackageVersion}\t" +
                    $"{manifest.ReleaseType}\t{Path.GetDirectoryName(Path.GetDirectoryName(sdk.PackageManifestPath))}\t" +
                    $"{sdk.ClangPath}");
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
