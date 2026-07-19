[TestFixture]
public class AngleSharpExtensionsTests
{
    static INodeList Parse(string html)
    {
        var parser = new HtmlParser();
        var document = parser.ParseDocument("<html><body></body></html>");
        return parser.ParseFragment(html, document.Body!);
    }

    [Test]
    public void DescendantsAndSelfDoesNotDuplicateTopLevelNodes()
    {
        var nodes = Parse("<a><b></b></a>");

        var result = nodes.DescendantsAndSelf().ToList();

        Assert.That(result.Select(_ => _.NodeName), Is.EqualTo(new[] { "A", "B" }));
    }

    [Test]
    public void DescendantsAndSelfDoesNotDuplicateSiblings()
    {
        var nodes = Parse("<p>one</p><p>two</p>");

        var result = nodes.DescendantsAndSelf<IElement>().ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Distinct().Count(), Is.EqualTo(2));
    }
}
