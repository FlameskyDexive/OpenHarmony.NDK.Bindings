using System.Runtime.InteropServices;
using OpenHarmony.NDK.Bindings.Native.Generated.ComponentInput;
using OpenHarmony.NDK.Bindings.Native.Generated.IPC;
using OpenHarmony.NDK.Bindings.Native.Manual.IPC;
using OpenHarmony.NDK.Bindings.Native.Manual.NodeApi;

namespace OpenHarmony.NDK.Bindings.Tests;

public sealed unsafe class IpcNodeComponentTests
{
    [Fact]
    public void Callback_lifetimes_retain_delegate_targets_until_disposed()
    {
        static int Request(uint _, nint __, nint ___, nint ____) => 0;
        static void Destroy(nint _) { }
        using IpcCallbackLifetime ipc = new(
            (code, data, reply, userData) => Request(code, (nint)data, (nint)reply, userData),
            Destroy);
        using NapiCallbackLifetime napi = new((Action)(() => { }));

        Assert.NotEqual(0, ipc.RequestFunction);
        Assert.NotEqual(0, ipc.DestroyFunction);
        Assert.NotEqual(0, napi.FunctionPointer);
    }

    [Fact]
    public void Ipc_message_option_matches_pack4_native_layout()
    {
        Assert.Equal(0, Marshal.OffsetOf<IpcMessageOption>(nameof(IpcMessageOption.Mode)).ToInt32());
        Assert.Equal(4, Marshal.OffsetOf<IpcMessageOption>(nameof(IpcMessageOption.Timeout)).ToInt32());
        Assert.Equal(8, Marshal.OffsetOf<IpcMessageOption>(nameof(IpcMessageOption.Reserved)).ToInt32());
        Assert.Equal(IntPtr.Size == 8 ? 16 : 12, Marshal.SizeOf<IpcMessageOption>());
    }

    [Fact]
    public void XComponent_callback_layout_is_four_pointer_slots()
    {
        Assert.Equal(IntPtr.Size * 4, Marshal.SizeOf<XComponentCallback>());
    }

    [Fact]
    public void Generated_input_surface_contains_key_event_lifecycle()
    {
        Assert.NotNull(typeof(InputNative).GetMethod(nameof(InputNative.CreateKeyEvent)));
        Assert.NotNull(typeof(InputNative).GetMethod(nameof(InputNative.DestroyKeyEvent)));
    }
}
