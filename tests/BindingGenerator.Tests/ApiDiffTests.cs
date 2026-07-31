using OpenHarmony.Ndk.Bindings.Generator.Diff;
using OpenHarmony.Ndk.Bindings.Generator.Inventory;

namespace OpenHarmony.Ndk.Bindings.Generator.Tests;

public sealed class ApiDiffTests
{
    [Fact]
    public void Compare_reports_added_removed_and_changed_headers_in_order()
    {
        HeaderInventoryDocument before = new(1, 15, "before", new[]
        {
            new HeaderRecord("a.h", 1, "old-a"),
            new HeaderRecord("b.h", 1, "old-b"),
            new HeaderRecord("c.h", 1, "old-c")
        });
        HeaderInventoryDocument after = new(1, 26, "after", new[]
        {
            new HeaderRecord("a.h", 2, "new-a"),
            new HeaderRecord("c.h", 1, "old-c"),
            new HeaderRecord("d.h", 1, "new-d")
        });

        ApiDiffDocument diff = ApiDiff.Compare(before, after);

        Assert.Equal(15, diff.BeforeApi);
        Assert.Equal(26, diff.AfterApi);
        Assert.Equal("d.h", Assert.Single(diff.Added).Path);
        Assert.Equal("b.h", Assert.Single(diff.Removed).Path);
        Assert.Equal("a.h", Assert.Single(diff.Changed).Path);
    }
}
