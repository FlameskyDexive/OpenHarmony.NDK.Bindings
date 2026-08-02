namespace OpenHarmony.Ndk.Bindings.Generator.Sdk;

public static class ApiAvailability
{
    public const string Api25Error =
        "API 25 is intentionally unsupported because no compatible SDK/image is available.";

    public static IReadOnlyList<int> SupportedApis { get; } = Enumerable.Range(13, 12).Append(26).ToArray();
    public static IReadOnlyList<int> NativeBuildApis { get; } = new[] { 13, 14, 15, 18, 20, 23, 26 };
    public static IReadOnlyList<int> NativeUnavailableApis { get; } = new[] { 16, 17, 19, 21, 22, 24 };

    public static bool IsSupported(int apiLevel) => SupportedApis.Contains(apiLevel);

    public static void RequireSupported(int apiLevel)
    {
        if (apiLevel == 25)
        {
            throw new ArgumentException(Api25Error);
        }

        if (!IsSupported(apiLevel))
        {
            throw new ArgumentException(
                $"Unsupported HarmonyOS API level '{apiLevel}'. Supported APIs are 13 through 24 and 26.");
        }
    }
}
