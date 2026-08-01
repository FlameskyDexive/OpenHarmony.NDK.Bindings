using System.Runtime.InteropServices;
using System.Text;
using OpenHarmony.NDK.Bindings.Compatibility;
using OpenHarmony.NDK.Bindings.Native.Generated.Foundation;

namespace OpenHarmony.NDK.Bindings.Native.Manual.HiLog;

public static unsafe class HiLogSafe
{
    public static int Write(int targetApi, HiLogLevel level, uint domain, string tag, string message)
    {
        ApiAvailability.Require(targetApi, 18, "OH_LOG_PrintMsg");
        ArgumentNullException.ThrowIfNull(tag);
        ArgumentNullException.ThrowIfNull(message);
        byte[] tagBytes = Encoding.UTF8.GetBytes(tag + "\0");
        byte[] messageBytes = Encoding.UTF8.GetBytes(message + "\0");
        fixed (byte* tagPointer = tagBytes)
        fixed (byte* messagePointer = messageBytes)
        {
            return HiLogNative.PrintMessage(HiLogType.App, level, domain, (sbyte*)tagPointer, (sbyte*)messagePointer);
        }
    }
}
