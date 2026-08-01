using Microsoft.Win32.SafeHandles;
using OpenHarmony.NDK.Bindings.Native.Generated.IPC;

namespace OpenHarmony.NDK.Bindings.Native.Manual.IPC;

public sealed unsafe class IpcParcelHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private IpcParcelHandle() : base(ownsHandle: true) { }

    public static IpcParcelHandle Create()
    {
        OHIPCParcel* parcel = IpcNative.CreateParcel();
        if (parcel is null) throw new InvalidOperationException("OH_IPCParcel_Create returned null.");
        IpcParcelHandle handle = new();
        handle.SetHandle((nint)parcel);
        return handle;
    }

    public OHIPCParcel* DangerousGetParcel() => (OHIPCParcel*)handle;

    protected override bool ReleaseHandle()
    {
        IpcNative.DestroyParcel((OHIPCParcel*)handle);
        return true;
    }
}
