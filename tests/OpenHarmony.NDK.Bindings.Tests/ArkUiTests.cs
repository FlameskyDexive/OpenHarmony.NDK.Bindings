using System.Runtime.InteropServices;
using OpenHarmony.NDK.Bindings.Native.Generated.ArkUI;

namespace OpenHarmony.NDK.Bindings.Tests;

public sealed class ArkUiTests
{
    [Fact]
    public void Accessibility_layouts_match_API13_plus_native_contract()
    {
        Assert.Equal(16, Marshal.SizeOf<ArkUiAccessibleRect>());
        Assert.Equal(24, Marshal.SizeOf<ArkUiAccessibleRangeInfo>());
        Assert.Equal(12, Marshal.SizeOf<ArkUiAccessibleGridInfo>());
        Assert.Equal(20, Marshal.SizeOf<ArkUiAccessibleGridItemInfo>());
    }

    [Fact]
    public void Versioned_node_api_view_does_not_flatten_unknown_fields()
    {
        Assert.Equal(32, Marshal.SizeOf<ArkUiNativeNodeApiV1Prefix>());
        Assert.Equal(8, Marshal.OffsetOf<ArkUiNativeNodeApiV1Prefix>(nameof(ArkUiNativeNodeApiV1Prefix.CreateNode)).ToInt32());
        Assert.Equal(16, Marshal.OffsetOf<ArkUiNativeNodeApiV1Prefix>(nameof(ArkUiNativeNodeApiV1Prefix.DisposeNode)).ToInt32());
        Assert.Equal(0, (int)ArkUiNativeApiVariantKind.Node);
        Assert.True(typeof(ArkUiNativeNodeApiView).IsValueType);
    }
}
