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
    public async Task UnwrapsSingleElementChildInPlace()
    {
        var result = Scrub("<div><p>content</p></div><footer>foot</footer>");

        await Assert.That(result).IsEqualTo("<p>content</p><footer>foot</footer>");
    }

    [Test]
    public async Task UnwrapsElementSurroundedByWhitespace()
    {
        var result = Scrub("<div>\n  <p>content</p>\n</div><footer>foot</footer>");

        await Assert.That(result).IsEqualTo("<p>content</p><footer>foot</footer>");
    }

    [Test]
    public async Task UnwrapsNestedDivs()
    {
        var result = Scrub("<div><div><p>deep</p></div></div><footer>foot</footer>");

        await Assert.That(result).IsEqualTo("<p>deep</p><footer>foot</footer>");
    }

    [Test]
    public async Task DoesNotUnwrapWhenTextWouldBeLost()
    {
        var result = Scrub("<div>hello <span>world</span></div>");

        await Assert.That(result).IsEqualTo("<div>hello <span>world</span></div>");
    }

    [Test]
    public async Task DoesNotUnwrapMultipleElementChildren()
    {
        var result = Scrub("<div><p>a</p><p>b</p></div>");

        await Assert.That(result).IsEqualTo("<div><p>a</p><p>b</p></div>");
    }

    [Test]
    public async Task RemovesEmptyDiv()
    {
        var result = Scrub("<div></div><footer>foot</footer>");

        await Assert.That(result).IsEqualTo("<footer>foot</footer>");
    }

    [Test]
    public async Task RemovesWhitespaceOnlyDiv()
    {
        var result = Scrub("<div>\n   </div><footer>foot</footer>");

        await Assert.That(result).IsEqualTo("<footer>foot</footer>");
    }

    [Test]
    public async Task KeepsTextOnlyDiv()
    {
        var result = Scrub("<div>My First Heading</div>");

        await Assert.That(result).IsEqualTo("<div>My First Heading</div>");
    }

    static IElement FirstElement(string html)
    {
        var parser = new HtmlParser();
        var document = parser.ParseDocument($"<html><body>{html}</body></html>");
        return document.Body!.Children[0];
    }

    [Test]
    public async Task ReturnsTrueWhenDivRemoved() =>
        await Assert.That(FirstElement("<div></div>").TryScrubDiv()).IsTrue();

    [Test]
    public async Task ReturnsTrueWhenDivUnwrapped() =>
        await Assert.That(FirstElement("<div><p>x</p></div>").TryScrubDiv()).IsTrue();

    [Test]
    public async Task ReturnsFalseForNonDiv() =>
        await Assert.That(FirstElement("<p>x</p>").TryScrubDiv()).IsFalse();

    [Test]
    public async Task ReturnsFalseWhenDivHasAttributes() =>
        await Assert.That(FirstElement("<div id='a'><p>x</p></div>").TryScrubDiv()).IsFalse();

    [Test]
    public async Task ReturnsFalseWhenNothingToScrub() =>
        await Assert.That(FirstElement("<div><p>a</p><p>b</p></div>").TryScrubDiv()).IsFalse();

    [Test]
    public async Task DoesNotThrowForDivWithoutParent()
    {
        var parser = new HtmlParser();
        var document = parser.ParseDocument("<html><body><div><p>content</p></div></body></html>");
        var div = document.QuerySelector("div")!;
        div.Remove();

        await Assert.That(() => div.TryScrubDiv()).ThrowsNothing();
    }

    [Test]
    public async Task KeepsDivWithAttributes()
    {
        var result = Scrub("<div id='keep'><p>content</p></div>");

        await Assert.That(result).IsEqualTo("""<div id="keep"><p>content</p></div>""");
    }
}
