using System.Runtime.InteropServices;
using System.Text.Json;
using OpenHarmony.NDK.Bindings.Native.Generated.ArkUI;
using OpenHarmony.NDK.Bindings.Native.Generated.Foundation;

var output = args.Length >= 2 && args[0] == "--output" ? args[1] : "abi-managed.json";
var compare = args.Length >= 2 && args[0] == "--compare" ? args[1] : null;
var document = new
{
    schemaVersion = 1,
    rawFileDescriptor = new { size = Marshal.SizeOf<RawFileDescriptor>(), align = 8, fdOffset = 0, startOffset = 8, lengthOffset = 16 },
    nativeBufferConfig = new { size = Marshal.SizeOf<NativeBufferConfig>(), align = 4, strideOffset = 16 },
    nativeBufferPlane = new { size = Marshal.SizeOf<NativeBufferPlane>(), align = 8, rowStrideOffset = 8 },
    accessibleRect = new { size = Marshal.SizeOf<ArkUiAccessibleRect>(), align = 4, rightBottomXOffset = 8 },
    accessibleRange = new { size = Marshal.SizeOf<ArkUiAccessibleRangeInfo>(), align = 8, currentOffset = 16 },
    accessibleGrid = new { size = Marshal.SizeOf<ArkUiAccessibleGridInfo>(), align = 4, selectionModeOffset = 8 },
    accessibleGridItem = new { size = Marshal.SizeOf<ArkUiAccessibleGridItemInfo>(), align = 4, columnIndexOffset = 4 }
};
Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);
File.WriteAllText(output, JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true }) + Environment.NewLine);
if (compare is not null)
{
    using JsonDocument expected = JsonDocument.Parse(JsonSerializer.Serialize(document));
    using JsonDocument actual = JsonDocument.Parse(File.ReadAllText(compare));
    foreach (string name in new[] { "rawFileDescriptor", "nativeBufferConfig", "nativeBufferPlane", "accessibleRect", "accessibleRange", "accessibleGrid", "accessibleGridItem" })
    {
        if (!actual.RootElement.TryGetProperty(name, out JsonElement actualValue) ||
            !JsonElementDeepEquals(expected.RootElement.GetProperty(name), actualValue))
            throw new InvalidDataException($"ABI mismatch in {name}: expected managed layout, native probe differs.");
    }
}
Console.WriteLine(output);

static bool JsonElementDeepEquals(JsonElement left, JsonElement right)
{
    if (left.ValueKind != right.ValueKind) return false;
    if (left.ValueKind == JsonValueKind.Object)
    {
        foreach (JsonProperty property in left.EnumerateObject())
        {
            if (!right.TryGetProperty(property.Name, out JsonElement rightProperty) || !JsonElementDeepEquals(property.Value, rightProperty)) return false;
        }
        return true;
    }
    return left.GetRawText() == right.GetRawText();
}
