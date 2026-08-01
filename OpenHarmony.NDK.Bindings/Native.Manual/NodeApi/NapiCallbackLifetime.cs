using System.Runtime.InteropServices;

namespace OpenHarmony.NDK.Bindings.Native.Manual.NodeApi;

public sealed class NapiCallbackLifetime : IDisposable
{
    private readonly Delegate _callback;
    private bool _disposed;

    public NapiCallbackLifetime(Delegate callback) => _callback = callback ?? throw new ArgumentNullException(nameof(callback));

    public nint FunctionPointer => Marshal.GetFunctionPointerForDelegate(_callback);

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        GC.KeepAlive(_callback);
        GC.SuppressFinalize(this);
    }
}
