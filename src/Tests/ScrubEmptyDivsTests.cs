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
    public void UnwrapsSingleElementChildInPlace()
    {
        var result = Scrub("<div><p>content</p></div><footer>foot</footer>");

        Assert.That(result, Is.EqualTo("<p>content</p><footer>foot</footer>"));
    }

    [Test]
    public void UnwrapsElementSurroundedByWhitespace()
    {
        var result = Scrub("<div>\n  <p>content</p>\n</div><footer>foot</footer>");

        Assert.That(result, Is.EqualTo("<p>content</p><footer>foot</footer>"));
    }

    [Test]
    public void UnwrapsNestedDivs()
    {
        var result = Scrub("<div><div><p>deep</p></div></div><footer>foot</footer>");

        Assert.That(result, Is.EqualTo("<p>deep</p><footer>foot</footer>"));
    }

    [Test]
    public void DoesNotUnwrapWhenTextWouldBeLost()
    {
        var result = Scrub("<div>hello <span>world</span></div>");

        Assert.That(result, Is.EqualTo("<div>hello <span>world</span></div>"));
    }

    [Test]
    public void DoesNotUnwrapMultipleElementChildren()
    {
        var result = Scrub("<div><p>a</p><p>b</p></div>");

        Assert.That(result, Is.EqualTo("<div><p>a</p><p>b</p></div>"));
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

    static IElement FirstElement(string html)
    {
        var parser = new HtmlParser();
        var document = parser.ParseDocument($"<html><body>{html}</body></html>");
        return document.Body!.Children[0];
    }

    [Test]
    public void ReturnsTrueWhenDivRemoved() =>
        Assert.That(FirstElement("<div></div>").TryScrubDiv(), Is.True);

    [Test]
    public void ReturnsTrueWhenDivUnwrapped() =>
        Assert.That(FirstElement("<div><p>x</p></div>").TryScrubDiv(), Is.True);

    [Test]
    public void ReturnsFalseForNonDiv() =>
        Assert.That(FirstElement("<p>x</p>").TryScrubDiv(), Is.False);

    [Test]
    public void ReturnsFalseWhenDivHasAttributes() =>
        Assert.That(FirstElement("<div id='a'><p>x</p></div>").TryScrubDiv(), Is.False);

    [Test]
    public void ReturnsFalseWhenNothingToScrub() =>
        Assert.That(FirstElement("<div><p>a</p><p>b</p></div>").TryScrubDiv(), Is.False);

    [Test]
    public void DoesNotThrowForDivWithoutParent()
    {
        var parser = new HtmlParser();
        var document = parser.ParseDocument("<html><body><div><p>content</p></div></body></html>");
        var div = document.QuerySelector("div")!;
        div.Remove();

        Assert.DoesNotThrow(() => div.TryScrubDiv());
    }

    [Test]
    public void KeepsDivWithAttributes()
    {
        var result = Scrub("<div id='keep'><p>content</p></div>");

        Assert.That(result, Is.EqualTo("""<div id="keep"><p>content</p></div>"""));
    }
}
