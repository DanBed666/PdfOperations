namespace PdfOperations.Tests;

[TestClass]
public class PagesRangeBuilderTests
{
    [TestMethod]
    public void BuildEvenPagesTests()
    {
        string pages = PagesRangeBuilder.BuildEvenPages(8);
        Assert.AreEqual("2,4,6,8", pages);
    }

    [TestMethod]
    public void BuildOddPagesTests()
    {
        string pages = PagesRangeBuilder.BuildOddPages(7);
        Assert.AreEqual("1,3,5,7", pages);
    }
}