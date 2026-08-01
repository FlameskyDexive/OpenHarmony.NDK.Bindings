using System.Runtime.CompilerServices;
using OpenHarmony.NDK.Bindings.Compatibility;
using OpenHarmony.NDK.Bindings.Native.Generated.Foundation;

namespace OpenHarmony.NDK.Bindings.Tests;

public sealed class FoundationTests
{
    [Fact]
    public void Availability_covers_requested_harmony_matrix()
    {
        Assert.True(ApiAvailability.IsSupported(15));
        Assert.True(ApiAvailability.IsSupported(18));
        Assert.True(ApiAvailability.IsSupported(20));
        Assert.True(ApiAvailability.IsSupported(23));
        Assert.True(ApiAvailability.IsSupported(26));
        Assert.False(ApiAvailability.IsSupported(14));
        Assert.True(ApiAvailability.IsAvailable(20, 18));
        Assert.False(ApiAvailability.IsAvailable(15, 18));
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
}
