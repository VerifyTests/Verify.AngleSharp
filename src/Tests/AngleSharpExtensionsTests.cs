public class AngleSharpExtensionsTests
{
    static INodeList Parse(string html)
    {
        var parser = new HtmlParser();
        var document = parser.ParseDocument("<html><body></body></html>");
        return parser.ParseFragment(html, document.Body!);
    }

    [Test]
    public async Task DescendantsAndSelfDoesNotDuplicateTopLevelNodes()
    {
        var nodes = Parse("<a><b></b></a>");

        var result = nodes.DescendantsAndSelf().ToList();

        await Assert.That(result.Select(_ => _.NodeName)).IsEquivalentTo(["A", "B"], CollectionOrdering.Matching);
    }

    [Test]
    public async Task DescendantsAndSelfDoesNotDuplicateSiblings()
    {
        var nodes = Parse("<p>one</p><p>two</p>");

        var result = nodes.DescendantsAndSelf<IElement>().ToList();

        await Assert.That(result).Count().IsEqualTo(2);
        await Assert.That(result.Distinct().Count()).IsEqualTo(2);
    }

    [Test]
    public async Task DescendantsExcludesTopLevelNodes()
    {
        var nodes = Parse("<a><b></b></a>");

        var result = nodes.Descendants().ToList();

        await Assert.That(result.Select(_ => _.NodeName)).IsEquivalentTo(["B"], CollectionOrdering.Matching);
    }

    [Test]
    public async Task DescendantsOfTypeExcludesTopLevelNodes()
    {
        var nodes = Parse("<p>one</p><p>two</p>");

        var result = nodes.Descendants<IElement>().ToList();

        await Assert.That(result).IsEmpty();
    }
}
