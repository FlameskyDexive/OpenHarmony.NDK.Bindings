using System.Runtime.InteropServices;

namespace OpenHarmony.NDK.Bindings.Native.Generated.Foundation;

[StructLayout(LayoutKind.Sequential)] public struct OH_NativeBuffer { }

[StructLayout(LayoutKind.Sequential)]
public struct NativeBufferConfig
{
    public int Width;
    public int Height;
    public int Format;
    public int Usage;
    public int Stride;
}

[StructLayout(LayoutKind.Sequential)]
public struct NativeBufferPlane
{
    public ulong Offset;
    public uint RowStride;
    public uint ColumnStride;
}

public static unsafe partial class NativeBufferNative
{
    [LibraryImport("libnative_buffer.so", EntryPoint = "OH_NativeBuffer_Alloc")]
    public static partial OH_NativeBuffer* Alloc(NativeBufferConfig* config);

    [LibraryImport("libnative_buffer.so", EntryPoint = "OH_NativeBuffer_Reference")]
    public static partial int Reference(OH_NativeBuffer* buffer);

    [LibraryImport("libnative_buffer.so", EntryPoint = "OH_NativeBuffer_Unreference")]
    public static partial int Unreference(OH_NativeBuffer* buffer);

    [LibraryImport("libnative_buffer.so", EntryPoint = "OH_NativeBuffer_GetConfig")]
    public static partial void GetConfig(OH_NativeBuffer* buffer, NativeBufferConfig* config);

    [LibraryImport("libnative_buffer.so", EntryPoint = "OH_NativeBuffer_Map")]
    public static partial int Map(OH_NativeBuffer* buffer, void** virtualAddress);

    [LibraryImport("libnative_buffer.so", EntryPoint = "OH_NativeBuffer_Unmap")]
    public static partial int Unmap(OH_NativeBuffer* buffer);

    [LibraryImport("libnative_buffer.so", EntryPoint = "OH_NativeBuffer_GetSeqNum")]
    public static partial uint GetSequenceNumber(OH_NativeBuffer* buffer);
}
