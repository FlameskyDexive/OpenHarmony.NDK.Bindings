using System.Runtime.InteropServices;

namespace OpenHarmony.NDK.Bindings.Native.Generated.Foundation;

[StructLayout(LayoutKind.Sequential)] public struct OH_NativeImage { }
[StructLayout(LayoutKind.Sequential)] public struct OHNativeWindow { }

public static unsafe partial class NativeImageNative
{
    [LibraryImport("libnative_image.so", EntryPoint = "OH_NativeImage_Create")]
    public static partial OH_NativeImage* Create(uint textureId, uint textureTarget);

    [LibraryImport("libnative_image.so", EntryPoint = "OH_NativeImage_AcquireNativeWindow")]
    public static partial OHNativeWindow* AcquireNativeWindow(OH_NativeImage* image);

    [LibraryImport("libnative_image.so", EntryPoint = "OH_NativeImage_UpdateSurfaceImage")]
    public static partial int UpdateSurfaceImage(OH_NativeImage* image);

    [LibraryImport("libnative_image.so", EntryPoint = "OH_NativeImage_GetTimestamp")]
    public static partial long GetTimestamp(OH_NativeImage* image);

    [LibraryImport("libnative_image.so", EntryPoint = "OH_NativeImage_GetTransformMatrix")]
    public static partial int GetTransformMatrix(OH_NativeImage* image, float* matrix16);

    [LibraryImport("libnative_image.so", EntryPoint = "OH_NativeImage_GetSurfaceId")]
    public static partial int GetSurfaceId(OH_NativeImage* image, ulong* surfaceId);

    [LibraryImport("libnative_image.so", EntryPoint = "OH_NativeImage_UnsetOnFrameAvailableListener")]
    public static partial int UnsetOnFrameAvailableListener(OH_NativeImage* image);

    [LibraryImport("libnative_image.so", EntryPoint = "OH_NativeImage_Destroy")]
    public static partial void Destroy(OH_NativeImage** image);
}
