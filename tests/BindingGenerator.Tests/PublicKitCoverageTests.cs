using OpenHarmony.Ndk.Bindings.Generator.Coverage;

namespace OpenHarmony.Ndk.Bindings.Generator.Tests;

public sealed class PublicKitCoverageTests
{
    [Fact]
    public void Scan_requires_every_fixture_root_to_be_covered_or_explained()
    {
        string root = Path.Combine(Path.GetTempPath(), "openharmony-kit-test-" + Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(Path.Combine(root, "usr", "include", "public"));
            Directory.CreateDirectory(Path.Combine(root, "usr", "include", "kernel"));
            PublicKitAllowlist allowlist = new(26, new[] { "public" }, new[] { new ExcludedHeaderRoot("kernel", "kernel-only") });
            PublicKitCoverageReport report = PublicKitCoverage.Scan(root, allowlist);
            Assert.Empty(report.MissingHeaderRoots);
            Assert.Empty(report.UnexplainedHeaderRoots);
            Assert.Equal("kernel-only", Assert.Single(report.ExcludedHeaderRoots).Reason);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
