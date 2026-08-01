using System.Runtime.InteropServices;
using OpenHarmony.NDK.Bindings.Native;

namespace OpenHarmony.NDK.Bindings.Native.Generated.NodeApi;

public static unsafe partial class NodeApiNative
{
    [LibraryImport("libace_napi.z.so", EntryPoint = "napi_create_function")]
    public static partial napi_status CreateFunction(napi_env env, sbyte* name, nuint nameLength,
        delegate* unmanaged[Cdecl]<napi_env, napi_callback_info, napi_value> callback, void* data, napi_value* result);

    [LibraryImport("libace_napi.z.so", EntryPoint = "napi_call_function")]
    public static partial napi_status CallFunction(napi_env env, napi_value recv, napi_value function,
        nuint argumentCount, napi_value* arguments, napi_value* result);

    [LibraryImport("libace_napi.z.so", EntryPoint = "napi_add_finalizer")]
    public static partial napi_status AddFinalizer(napi_env env, napi_value jsObject, void* nativeObject,
        delegate* unmanaged[Cdecl]<napi_env, void*, void*, void> finalizeCallback, void* finalizeHint, napi_ref* result);
}
