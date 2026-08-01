using System.Runtime.InteropServices;
using OpenHarmony.NDK.Bindings.Native.Generated.IPC;

namespace OpenHarmony.NDK.Bindings.Native.Manual.IPC;

public sealed unsafe class IpcCallbackLifetime : IDisposable
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int RequestCallback(uint code, OHIPCParcel* data, OHIPCParcel* reply, nint userData);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DestroyCallback(nint userData);

    private readonly RequestCallback _request;
    private readonly DestroyCallback _destroy;
    private bool _disposed;

    public IpcCallbackLifetime(RequestCallback request, DestroyCallback destroy)
    {
        _request = request ?? throw new ArgumentNullException(nameof(request));
        _destroy = destroy ?? throw new ArgumentNullException(nameof(destroy));
    }

    public nint RequestFunction => Marshal.GetFunctionPointerForDelegate(_request);
    public nint DestroyFunction => Marshal.GetFunctionPointerForDelegate(_destroy);

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        GC.KeepAlive(_request);
        GC.KeepAlive(_destroy);
        GC.SuppressFinalize(this);
    }
}
