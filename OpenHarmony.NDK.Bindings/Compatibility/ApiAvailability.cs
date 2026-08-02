namespace OpenHarmony.NDK.Bindings.Compatibility;

public static class ApiAvailability
{
    private static readonly int[] SupportedApiValues = Enumerable.Range(13, 12).Append(26).ToArray();
    private static readonly int[] NativeBuildApiValues = { 13, 14, 15, 18, 20, 23, 26 };
    private static readonly int[] NativeUnavailableApiValues = { 16, 17, 19, 21, 22, 24 };

    public const int MinimumApi = 13;
    public static ReadOnlySpan<int> SupportedApis => SupportedApiValues;
    public static IReadOnlyList<int> NativeBuildApis => NativeBuildApiValues;
    public static IReadOnlyList<int> NativeUnavailableApis => NativeUnavailableApiValues;

    public static bool IsSupported(int apiLevel) => SupportedApiValues.Contains(apiLevel);

    public static bool IsNativeSdkAvailable(int apiLevel) => NativeBuildApiValues.Contains(apiLevel);

    public static bool IsAvailable(int targetApi, int introducedApi) =>
        IsSupported(targetApi) && targetApi >= introducedApi;

    public static void Require(int targetApi, int introducedApi, string symbol)
    {
        if (targetApi == 25)
        {
            throw new ArgumentException(
                "API 25 is intentionally unsupported because no compatible SDK/image is available.");
        }
        if (!IsSupported(targetApi))
            throw new ArgumentOutOfRangeException(nameof(targetApi), targetApi, "Supported HarmonyOS APIs are 13 through 24 and 26.");
        if (targetApi < introducedApi)
            throw new PlatformNotSupportedException($"{symbol} requires HarmonyOS API {introducedApi}; target API is {targetApi}.");
    }
}
