using System.Runtime.CompilerServices;
using OpenHarmony.NDK.Bindings.Compatibility;
using OpenHarmony.NDK.Bindings.Native.Generated.Foundation;

namespace OpenHarmony.NDK.Bindings.Tests;

public sealed class FoundationTests
{
    [Fact]
    public void Availability_covers_requested_harmony_matrix()
    {
        Assert.Equal(13, ApiAvailability.MinimumApi);
        Assert.Equal(Enumerable.Range(13, 12).Append(26), ApiAvailability.SupportedApis.ToArray());
        Assert.Equal(
            new[] { 13, 14, 15, 18, 20, 23, 26 },
            GetApiSet("NativeBuildApis"));
        Assert.Equal(
            new[] { 16, 17, 19, 21, 22, 24 },
            GetApiSet("NativeUnavailableApis"));
        Assert.False(ApiAvailability.IsSupported(25));
        Assert.True(ApiAvailability.IsAvailable(20, 18));
        Assert.False(ApiAvailability.IsAvailable(15, 18));
    }

    [Fact]
    public void Availability_rejects_api25_with_the_exact_message()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => ApiAvailability.Require(25, 13, "symbol"));

        Assert.Equal(
            "API 25 is intentionally unsupported because no compatible SDK/image is available.",
            exception.Message.Split(Environment.NewLine, StringSplitOptions.None)[0]);
    }

    [Fact]
    public void Foundation_layouts_match_64_bit_native_contracts()
    {
        Assert.Equal(24, Unsafe.SizeOf<RawFileDescriptor>());
        Assert.Equal(20, Unsafe.SizeOf<NativeBufferConfig>());
        Assert.Equal(16, Unsafe.SizeOf<NativeBufferPlane>());
    }

    [Fact]
    public void Generated_foundation_surface_contains_api26_entry_points()
    {
        Assert.NotNull(typeof(NativeBufferNative).GetMethod(nameof(NativeBufferNative.GetSequenceNumber)));
        Assert.NotNull(typeof(RawFileNative).GetMethod(nameof(RawFileNative.GetRawFileDescriptorData)));
        Assert.NotNull(typeof(FileUriNative).GetMethod(nameof(FileUriNative.IsValidUri)));
        Assert.NotNull(typeof(ResourceManagerNative).GetMethod(nameof(ResourceManagerNative.OpenRawFile64)));
        Assert.NotNull(typeof(NativeImageNative).GetMethod(nameof(NativeImageNative.GetTransformMatrix)));
    }

    private static int[] GetApiSet(string propertyName)
    {
        object? value = typeof(ApiAvailability).GetProperty(propertyName)?.GetValue(null);
        Assert.NotNull(value);
        return ((IEnumerable<int>)value).ToArray();
    }
}
