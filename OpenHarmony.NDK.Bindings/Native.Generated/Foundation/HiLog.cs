using System.Runtime.InteropServices;

namespace OpenHarmony.NDK.Bindings.Native.Generated.Foundation;

public enum HiLogType { App = 0 }

public enum HiLogLevel { Debug = 3, Info = 4, Warn = 5, Error = 6, Fatal = 7 }

public static unsafe partial class HiLogNative
{
    [LibraryImport("libhilog_ndk.z.so", EntryPoint = "OH_LOG_PrintMsg")]
    public static partial int PrintMessage(HiLogType type, HiLogLevel level, uint domain, sbyte* tag, sbyte* message);

    [LibraryImport("libhilog_ndk.z.so", EntryPoint = "OH_LOG_PrintMsgByLen")]
    public static partial int PrintMessageByLength(HiLogType type, HiLogLevel level, uint domain, sbyte* tag, nuint tagLength, sbyte* message, nuint messageLength);
}
