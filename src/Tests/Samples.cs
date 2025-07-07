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

    #region ScrubAspCacheBusterTagHelper

    [Test]
    public Task ScrubAspCacheBusterTagHelper()
    {
        var html = """
                   <!DOCTYPE html>
                   <html>
                     <head>
                       <link
                        href="/css/site.css?v=r2K1aJs2_7mdAedOAb0OQw3K46ElgPZWqeuI"
                        rel="stylesheet"
                        />
                     </head>
                     <body>
                       <h1>My Heading</h1>
                     </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml(nodes => nodes.ScrubAspCacheBusterTagHelper());
    }

    #endregion

    #region ScrubBrowserLink

    [Test]
    public Task ScrubBrowserLink()
    {
        var html = """
                   <!DOCTYPE html>
                   <html>
                     <head>
                       <meta charset="utf-8">
                     </head>
                     <body>
                       <h1>My Heading</h1>


                       <!-- Visual Studio Browser Link -->
                       <script src="/_vs/browserLink">
                       </script>

                       <script src="/_framework/aspnetcore-browser-refresh.js">
                       </script>
                       <!-- End Browser Link -->

                       </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml(nodes => nodes.ScrubBrowserLink());
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

    #region SvgSample

    [Test]
    public Task SvgSample()
    {
        var svg =
            """
            <svg version="1.1"
                 width="300" height="200"
                 xmlns="http://www.w3.org/2000/svg">
              <rect width="100%" height="100%" fill="red" />
              <circle cx="150" cy="100" r="80" fill="green" />
              <text x="150" y="125"
                    font-size="60"
                    text-anchor="middle"
                    fill="white">
              SVG
              </text>
            </svg>
            """;
        return Verify(svg, "svg");
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

    static void CustomOptionsGlobal() =>
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