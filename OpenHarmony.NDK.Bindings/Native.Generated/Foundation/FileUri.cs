using System.Runtime.InteropServices;

namespace OpenHarmony.NDK.Bindings.Native.Generated.Foundation;

public enum FileManagementErrorCode
{
    Ok = 0
}

public static unsafe partial class FileUriNative
{
    [LibraryImport("libohfileuri.so", EntryPoint = "OH_FileUri_GetUriFromPath")]
    public static partial FileManagementErrorCode GetUriFromPath(sbyte* path, uint length, sbyte** result);

    [LibraryImport("libohfileuri.so", EntryPoint = "OH_FileUri_GetPathFromUri")]
    public static partial FileManagementErrorCode GetPathFromUri(sbyte* uri, uint length, sbyte** result);

    [LibraryImport("libohfileuri.so", EntryPoint = "OH_FileUri_IsValidUri")]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool IsValidUri(sbyte* uri, uint length);

    [LibraryImport("libohfileuri.so", EntryPoint = "OH_FileUri_GetFileName")]
    public static partial FileManagementErrorCode GetFileName(sbyte* uri, uint length, sbyte** result);
}
