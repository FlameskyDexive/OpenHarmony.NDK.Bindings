using System.Runtime.InteropServices;

namespace OpenHarmony.NDK.Bindings.Native.Generated.ComponentInput;

[StructLayout(LayoutKind.Sequential)] public struct OH_NativeXComponent { }

[StructLayout(LayoutKind.Sequential)]
public struct XComponentCallback
{
    public nint OnSurfaceCreated;
    public nint OnSurfaceChanged;
    public nint OnSurfaceDestroyed;
    public nint DispatchTouchEvent;
}

public static unsafe partial class XComponentNative
{
    [LibraryImport("libnative_xcomponent.so", EntryPoint = "OH_NativeXComponent_GetXComponentId")]
    public static partial int GetXComponentId(OH_NativeXComponent* component, sbyte* id, ulong* size);

    [LibraryImport("libnative_xcomponent.so", EntryPoint = "OH_NativeXComponent_GetXComponentSize")]
    public static partial int GetXComponentSize(OH_NativeXComponent* component, void* window, ulong* width, ulong* height);

    [LibraryImport("libnative_xcomponent.so", EntryPoint = "OH_NativeXComponent_GetTouchEvent")]
    public static partial int GetTouchEvent(OH_NativeXComponent* component, void* window, void* touchEvent);

    [LibraryImport("libnative_xcomponent.so", EntryPoint = "OH_NativeXComponent_RegisterCallback")]
    public static partial int RegisterCallback(OH_NativeXComponent* component, XComponentCallback* callback);
}
