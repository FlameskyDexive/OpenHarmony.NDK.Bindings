namespace OpenHarmony.Ndk.Bindings.Generator.Generation;

public static class ResponseFileWriter
{
    public static void Write(string outputPath, IEnumerable<string> headers, string sysroot)
    {
        string fullPath = Path.GetFullPath(outputPath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        string[] lines = new[]
        {
            "-fparse-all-comments",
            "-x",
            "c-header",
            $"-I{Quote(sysroot)}"
        }.Concat(headers
            .Select(Path.GetFullPath)
            .OrderBy(path => path, StringComparer.Ordinal)
            .Select(Quote))
            .ToArray();
        File.WriteAllLines(fullPath, lines);
    }

    private static string Quote(string value) => value.Contains(' ', StringComparison.Ordinal)
        ? $"\"{value.Replace("\\", "\\\\").Replace("\"", "\\\"")}\""
        : value;
}
