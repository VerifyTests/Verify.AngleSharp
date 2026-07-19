[TestFixture]
public class ScrubEmptyDivsTests
{
    static string Scrub(string html)
    {
        var parser = new HtmlParser();
        var document = parser.ParseDocument($"<html><body>{html}</body></html>");
        var body = document.Body!;
        body.ChildNodes.ScrubEmptyDivs();
        return body.InnerHtml;
    }

    [Test]
    public void RemovesEmptyDiv()
    {
        var result = Scrub("<div></div><footer>foot</footer>");

        Assert.That(result, Is.EqualTo("<footer>foot</footer>"));
    }

    [Test]
    public void RemovesWhitespaceOnlyDiv()
    {
        var result = Scrub("<div>\n   </div><footer>foot</footer>");

        Assert.That(result, Is.EqualTo("<footer>foot</footer>"));
    }

    [Test]
    public void KeepsTextOnlyDiv()
    {
        var result = Scrub("<div>My First Heading</div>");

        Assert.That(result, Is.EqualTo("<div>My First Heading</div>"));
    }

    [Test]
    public void KeepsDivWithAttributes()
    {
        var result = Scrub("<div id='keep'><p>content</p></div>");

        Assert.That(result, Is.EqualTo("""<div id="keep"><p>content</p></div>"""));
    }
}
