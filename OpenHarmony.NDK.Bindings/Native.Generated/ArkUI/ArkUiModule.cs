using System.Runtime.InteropServices;

namespace OpenHarmony.NDK.Bindings.Native.Generated.ArkUI;

public enum ArkUiNativeApiVariantKind
{
    Node = 0
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct ArkUiNativeNodeApiV1Prefix
{
    public int Version;
    public delegate* unmanaged[Cdecl]<int, nint> CreateNode;
    public delegate* unmanaged[Cdecl]<nint, void> DisposeNode;
    public delegate* unmanaged[Cdecl]<nint, nint, int> AddChild;
}

public unsafe readonly struct ArkUiNativeNodeApiView
{
    private readonly ArkUiNativeNodeApiV1Prefix* _api;

    internal ArkUiNativeNodeApiView(ArkUiNativeNodeApiV1Prefix* api) => _api = api;

    public int Version => _api->Version;
    public bool SupportsCreateNode => _api->Version >= 1 && _api->CreateNode != null;

    public nint CreateNode(int nodeType)
    {
        if (!SupportsCreateNode) throw new PlatformNotSupportedException("ArkUI native node API does not expose createNode.");
        return _api->CreateNode(nodeType);
    }

    public void DisposeNode(nint node)
    {
        if (_api->Version < 1 || _api->DisposeNode == null) throw new PlatformNotSupportedException("ArkUI native node API does not expose disposeNode.");
        _api->DisposeNode(node);
    }
}

public static unsafe partial class ArkUiModuleNative
{
    [LibraryImport("libace_ndk.z.so", EntryPoint = "OH_ArkUI_QueryModuleInterfaceByName")]
    private static partial void* QueryModuleInterfaceByName(ArkUiNativeApiVariantKind kind, sbyte* structName);

    public static bool TryQueryNodeApi(int targetApi, out ArkUiNativeNodeApiView view)
    {
        OpenHarmony.NDK.Bindings.Compatibility.ApiAvailability.Require(targetApi, 15, "ArkUI native node module");
        byte[] name = "ArkUI_NativeNodeAPI_1\0"u8.ToArray();
        fixed (byte* namePointer = name)
        {
            ArkUiNativeNodeApiV1Prefix* api = (ArkUiNativeNodeApiV1Prefix*)QueryModuleInterfaceByName(
                ArkUiNativeApiVariantKind.Node, (sbyte*)namePointer);
            view = api is null ? default : new ArkUiNativeNodeApiView(api);
            return api is not null;
        }
    }
}
