using System.Runtime.InteropServices;

namespace OpenHarmony.NDK.Bindings.Native.Generated.ArkUI;

[StructLayout(LayoutKind.Sequential)] public struct ArkUiAccessibleRect
{
    public int LeftTopX;
    public int LeftTopY;
    public int RightBottomX;
    public int RightBottomY;
}

[StructLayout(LayoutKind.Sequential)] public struct ArkUiAccessibleRangeInfo
{
    public double Min;
    public double Max;
    public double Current;
}

[StructLayout(LayoutKind.Sequential)] public struct ArkUiAccessibleGridInfo
{
    public int RowCount;
    public int ColumnCount;
    public int SelectionMode;
}

[StructLayout(LayoutKind.Sequential)] public struct ArkUiAccessibleGridItemInfo
{
    [MarshalAs(UnmanagedType.I1)] public bool Heading;
    [MarshalAs(UnmanagedType.I1)] public bool Selected;
    public int ColumnIndex;
    public int RowIndex;
    public int ColumnSpan;
    public int RowSpan;
}
