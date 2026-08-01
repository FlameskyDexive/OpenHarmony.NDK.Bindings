namespace OpenHarmony.NDK.Bindings.Compatibility;

public static class ApiAvailability
{
    public const int MinimumApi = 15;
    public static ReadOnlySpan<int> SupportedApis => new[] { 15, 18, 20, 23, 26 };

    public static bool IsSupported(int apiLevel) => SupportedApis.IndexOf(apiLevel) >= 0;

    public static bool IsAvailable(int targetApi, int introducedApi) =>
        IsSupported(targetApi) && targetApi >= introducedApi;

    public static void Require(int targetApi, int introducedApi, string symbol)
    {
        if (!IsSupported(targetApi))
            throw new ArgumentOutOfRangeException(nameof(targetApi), targetApi, "Supported HarmonyOS APIs are 15, 18, 20, 23, and 26.");
        if (targetApi < introducedApi)
            throw new PlatformNotSupportedException($"{symbol} requires HarmonyOS API {introducedApi}; target API is {targetApi}.");
    }
}
