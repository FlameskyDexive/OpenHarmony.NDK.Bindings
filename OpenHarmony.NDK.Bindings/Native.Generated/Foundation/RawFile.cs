using System.Runtime.InteropServices;

namespace OpenHarmony.NDK.Bindings.Native.Generated.Foundation;

[StructLayout(LayoutKind.Sequential)]
public struct RawFileDescriptor
{
    public int FileDescriptor;
    public long Start;
    public long Length;
}

[StructLayout(LayoutKind.Sequential)] public struct RawFile { }
[StructLayout(LayoutKind.Sequential)] public struct RawFile64 { }
[StructLayout(LayoutKind.Sequential)] public struct RawDir { }
[StructLayout(LayoutKind.Sequential)] public struct NativeResourceManager { }

public static unsafe partial class RawFileNative
{
    [LibraryImport("librawfile.z.so", EntryPoint = "OH_ResourceManager_OpenRawFile")]
    public static partial RawFile* OpenRawFile(NativeResourceManager* manager, sbyte* fileName);

    [LibraryImport("librawfile.z.so", EntryPoint = "OH_ResourceManager_ReadRawFile")]
    public static partial int ReadRawFile(RawFile* rawFile, void* buffer, nuint length);

    [LibraryImport("librawfile.z.so", EntryPoint = "OH_ResourceManager_SeekRawFile")]
    public static partial int SeekRawFile(RawFile* rawFile, nint offset, int whence);

    [LibraryImport("librawfile.z.so", EntryPoint = "OH_ResourceManager_GetRawFileSize")]
    public static partial nint GetRawFileSize(RawFile* rawFile);

    [LibraryImport("librawfile.z.so", EntryPoint = "OH_ResourceManager_CloseRawFile")]
    public static partial void CloseRawFile(RawFile* rawFile);

    [LibraryImport("librawfile.z.so", EntryPoint = "OH_ResourceManager_GetRawFileDescriptorData")]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool GetRawFileDescriptorData(RawFile* rawFile, RawFileDescriptor* descriptor);

    [LibraryImport("librawfile.z.so", EntryPoint = "OH_ResourceManager_ReleaseRawFileDescriptorData")]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool ReleaseRawFileDescriptorData(RawFileDescriptor* descriptor);
}
