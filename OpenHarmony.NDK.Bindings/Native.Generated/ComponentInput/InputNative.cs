using System.Runtime.InteropServices;

namespace OpenHarmony.NDK.Bindings.Native.Generated.ComponentInput;

[StructLayout(LayoutKind.Sequential)] public struct InputKeyEvent { }

public static unsafe partial class InputNative
{
    [LibraryImport("libohinput.so", EntryPoint = "OH_Input_CreateKeyEvent")]
    public static partial InputKeyEvent* CreateKeyEvent();

    [LibraryImport("libohinput.so", EntryPoint = "OH_Input_DestroyKeyEvent")]
    public static partial void DestroyKeyEvent(InputKeyEvent** keyEvent);

    [LibraryImport("libohinput.so", EntryPoint = "OH_Input_SetKeyEventAction")]
    public static partial void SetKeyEventAction(InputKeyEvent* keyEvent, int action);

    [LibraryImport("libohinput.so", EntryPoint = "OH_Input_GetKeyEventAction")]
    public static partial int GetKeyEventAction(InputKeyEvent* keyEvent);

    [LibraryImport("libohinput.so", EntryPoint = "OH_Input_SetKeyEventKeyCode")]
    public static partial void SetKeyEventKeyCode(InputKeyEvent* keyEvent, int keyCode);

    [LibraryImport("libohinput.so", EntryPoint = "OH_Input_GetKeyEventKeyCode")]
    public static partial int GetKeyEventKeyCode(InputKeyEvent* keyEvent);
}
