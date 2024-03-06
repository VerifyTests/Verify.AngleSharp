using AngleSharp.Diffing;
using AngleSharp.Diffing.Core;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using VerifyTests.AngleSharp;

[TestFixture]
public class Samples
{
    #region PrettyPrintHtml

    [Test]
    public Task PrettyPrintHtml()
    {
        var html = """
                   <!DOCTYPE html>
                   <html><body><h1>My First Heading</h1>
                   <p>My first paragraph.</p></body></html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml();
    }

    #endregion

    #region ScrubEmptyDivs

    [Test]
    public Task ScrubEmptyDivs()
    {
        var html = """
                   <!DOCTYPE html>
                   <html>
                     <body>
                       <div>My First Heading</div>
                       <div></div>
                     </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml(nodes => nodes.ScrubEmptyDivs());
    }

    #endregion

    #region ScrubAttributes

    [Test]
    public Task ScrubAttributes()
    {
        var html = """
                   <!DOCTYPE html>
                   <html>
                     <body>
                       <div id='a'>My First Heading</div>
                     </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml(nodes => nodes.ScrubAttributes("id"));
    }

    #endregion

    #region ScrubAttributeWithNewValue

    [Test]
    public Task ScrubAttributeWithNewValue()
    {
        var html = """
                   <!DOCTYPE html>
                   <html>
                     <body>
                       <div id='a'>My First Heading</div>
                     </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml(
                nodes => nodes.ScrubAttributes(x =>
                {
                    if (x.Name == "id")
                    {
                        return "new value";
                    }

                    return null;
                }));
    }

    #endregion

    #region PrettyPrintHtmlWithNodeManipulation

    [Test]
    public Task PrettyPrintHtmlWithNodeManipulation()
    {
        var html = """
                   <!DOCTYPE html>
                   <html>
                     <body>
                       <h1>My First Heading</h1>
                       <p>My first paragraph.</p>
                     </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml(
                nodes =>
                {
                    foreach (var node in nodes.QuerySelectorAll("h1"))
                    {
                        node.Remove();
                    }
                });
    }

    #endregion

    #region SingleMember

    [Test]
    public Task SingleMember()
    {
        var parser = new HtmlParser();
        var html =
            """
            <!DOCTYPE html>
            <html>
              <body>
                <h1>My First Heading</h1>
                <p>My first paragraph.</p>
              </body>
            </html>
            """;
        return Verify(
            new
            {
                HtmlProperty = parser.ParseDocument(html)
            });
    }

    #endregion

    #region Sample

    [Test]
    public Task Sample()
    {
        var html =
            """
            <!DOCTYPE html>
            <html>
              <body>
                <h1>My First Heading</h1>
                <p>My first paragraph.</p>
              </body>
            </html>
            """;
        return Verify(html, "html");
    }

    #endregion

    [Test]
    public Task CustomOptions()
    {
        #region CustomOptions

        var settings = new VerifySettings();
        settings.AngleSharpDiffingSettings(
            action =>
            {
                static FilterDecision SpanFilter(
                    in ComparisonSource source,
                    FilterDecision decision)
                {
                    if (source.Node.NodeName == "SPAN")
                    {
                        return FilterDecision.Exclude;
                    }

                    return decision;
                }

                var options = action.AddDefaultOptions();
                options.AddFilter(SpanFilter);
            });

        #endregion

        var html =
            """
            <!DOCTYPE html>
            <html>
              <body>
                <h1>My First Heading</h1>
                <p>My first paragraph.</p>
              </body>
            </html>
            """;
        return Verify(html, "html", settings);
    }

    static void CustomOptionsGlobal()
    {
        #region CustomOptionsGlobal

        VerifyAngleSharpDiffing.Initialize(
            action =>
            {
                static FilterDecision SpanFilter(
                    in ComparisonSource source,
                    FilterDecision decision)
                {
                    if (source.Node.NodeName == "SPAN")
                    {
                        return FilterDecision.Exclude;
                    }

                    return decision;
                }

                var options = action.AddDefaultOptions();
                options.AddFilter(SpanFilter);
            });

        #endregion
    }
}