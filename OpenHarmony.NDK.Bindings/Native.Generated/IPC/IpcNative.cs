using System.Runtime.InteropServices;

namespace OpenHarmony.NDK.Bindings.Native.Generated.IPC;

[StructLayout(LayoutKind.Sequential)] public struct OHIPCParcel { }
[StructLayout(LayoutKind.Sequential)] public struct OHIPCRemoteStub { }
[StructLayout(LayoutKind.Sequential)] public struct OHIPCRemoteProxy { }
[StructLayout(LayoutKind.Sequential)] public struct OHIPCDeathRecipient { }

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public unsafe struct IpcMessageOption
{
    public int Mode;
    public uint Timeout;
    public void* Reserved;
}

public static unsafe partial class IpcNative
{
    [LibraryImport("libipc_capi.so", EntryPoint = "OH_IPCParcel_Create")]
    public static partial OHIPCParcel* CreateParcel();

    [LibraryImport("libipc_capi.so", EntryPoint = "OH_IPCParcel_Destroy")]
    public static partial void DestroyParcel(OHIPCParcel* parcel);

    [LibraryImport("libipc_capi.so", EntryPoint = "OH_IPCParcel_WriteInt32")]
    public static partial int WriteInt32(OHIPCParcel* parcel, int value);

    [LibraryImport("libipc_capi.so", EntryPoint = "OH_IPCParcel_ReadInt32")]
    public static partial int ReadInt32(OHIPCParcel* parcel, int* value);

    [LibraryImport("libipc_capi.so", EntryPoint = "OH_IPCRemoteStub_Create")]
    public static partial OHIPCRemoteStub* CreateRemoteStub(sbyte* descriptor,
        delegate* unmanaged[Cdecl]<uint, OHIPCParcel*, OHIPCParcel*, void*, int> requestCallback,
        delegate* unmanaged[Cdecl]<void*, void> destroyCallback, void* userData);

    [LibraryImport("libipc_capi.so", EntryPoint = "OH_IPCRemoteStub_Destroy")]
    public static partial void DestroyRemoteStub(OHIPCRemoteStub* stub);

    [LibraryImport("libipc_capi.so", EntryPoint = "OH_IPCRemoteProxy_SendRequest")]
    public static partial int SendRequest(OHIPCRemoteProxy* proxy, uint code, OHIPCParcel* data,
        OHIPCParcel* reply, IpcMessageOption* option);
}
