using OpenHarmony.Ndk.Bindings.Generator.Inventory;

namespace OpenHarmony.Ndk.Bindings.Generator.Tests;

public sealed class HeaderInventoryTests
{
    [Fact]
    public void Scan_is_sorted_and_excludes_architecture_specific_roots()
    {
        using TemporaryHeaders fixture = TemporaryHeaders.Create();
        HeaderInventoryDocument inventory = HeaderInventory.Scan(fixture.Sysroot, 26);

        Assert.Equal(new[] { "AbilityKit/ability.h", "arkui/arkui.h" }, inventory.Headers.Select(header => header.Path));
        Assert.Equal(64, inventory.Headers[0].Sha256.Length);
        Assert.Equal(inventory.Headers.OrderBy(header => header.Path, StringComparer.Ordinal), inventory.Headers);
    }

    private sealed class TemporaryHeaders : IDisposable
    {
        private TemporaryHeaders(string root) => Sysroot = root;
        public string Sysroot { get; }

        public static TemporaryHeaders Create()
        {
            string root = Path.Combine(Path.GetTempPath(), "openharmony-header-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(root, "usr", "include", "AbilityKit"));
            Directory.CreateDirectory(Path.Combine(root, "usr", "include", "arkui"));
            Directory.CreateDirectory(Path.Combine(root, "usr", "include", "aarch64-linux-ohos"));
            File.WriteAllText(Path.Combine(root, "usr", "include", "AbilityKit", "ability.h"), "ability");
            File.WriteAllText(Path.Combine(root, "usr", "include", "arkui", "arkui.h"), "arkui");
            File.WriteAllText(Path.Combine(root, "usr", "include", "aarch64-linux-ohos", "arch.h"), "arch");
            return new TemporaryHeaders(root);
        }

        public void Dispose() => Directory.Delete(Sysroot, recursive: true);
    }
}
