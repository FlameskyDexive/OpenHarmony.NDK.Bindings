using System.Runtime.InteropServices;

namespace OpenHarmony.NDK.Bindings.Native.Generated.Foundation;

public static unsafe partial class ResourceManagerNative
{
    [LibraryImport("librawfile.z.so", EntryPoint = "OH_ResourceManager_InitNativeResourceManager")]
    public static partial NativeResourceManager* InitNativeResourceManager(IntPtr env, IntPtr jsResourceManager);

    [LibraryImport("librawfile.z.so", EntryPoint = "OH_ResourceManager_ReleaseNativeResourceManager")]
    public static partial void ReleaseNativeResourceManager(NativeResourceManager* manager);

    [LibraryImport("librawfile.z.so", EntryPoint = "OH_ResourceManager_OpenRawDir")]
    public static partial RawDir* OpenRawDir(NativeResourceManager* manager, sbyte* directoryName);

    [LibraryImport("librawfile.z.so", EntryPoint = "OH_ResourceManager_OpenRawFile64")]
    public static partial RawFile64* OpenRawFile64(NativeResourceManager* manager, sbyte* fileName);

    [LibraryImport("librawfile.z.so", EntryPoint = "OH_ResourceManager_IsRawDir")]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool IsRawDir(NativeResourceManager* manager, sbyte* path);

    [LibraryImport("librawfile.z.so", EntryPoint = "OH_ResourceManager_GetRawFileName")]
    public static partial sbyte* GetRawFileName(RawDir* rawDirectory, int index);

    [LibraryImport("librawfile.z.so", EntryPoint = "OH_ResourceManager_GetRawFileCount")]
    public static partial int GetRawFileCount(RawDir* rawDirectory);

    [LibraryImport("librawfile.z.so", EntryPoint = "OH_ResourceManager_CloseRawDir")]
    public static partial void CloseRawDir(RawDir* rawDirectory);
}
