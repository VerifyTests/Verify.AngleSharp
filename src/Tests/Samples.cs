using AngleSharp.Diffing;
using AngleSharp.Diffing.Core;
using AngleSharp.Dom;
using VerifyTests.AngleSharp;

[TestFixture]
public class Samples
{
    #region PrettyPrintHtml

    [Test]
    public Task PrettyPrintHtml()
    {
        var html = @"<!DOCTYPE html>
<html><body><h1>My First Heading</h1>
<p>My first paragraph.</p></body></html>";
        return Verify(html)
            .UseExtension("html")
            .PrettyPrintHtml();
    }

    #endregion

    [Test]
    public Task EmptyDiv()
    {
        var html = @"<!DOCTYPE html>
<html>
  <body>
    <div>My First Heading</div>
    <div></div>
  </body>
</html>";
        return Verify(html)
            .UseExtension("html")
            .PrettyPrintHtml(nodes => nodes.ScrubEmptyDivs());
    }

    #region PrettyPrintHtmlWithNodeManipulation

    [Test]
    public Task PrettyPrintHtmlWithNodeManipulation()
    {
        var html = @"<!DOCTYPE html>
<html>
  <body>
    <h1>My First Heading</h1>
    <p>My first paragraph.</p>
  </body>
</html>";
        return Verify(html)
            .UseExtension("html")
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

    #region Sample

    [Test]
    public Task Sample()
    {
        var html = @"<!DOCTYPE html>
<html>
<body>

<h1>My First Heading</h1>
<p>My first paragraph.</p>

</body>
</html>";
        return Verify(html)
            .UseExtension("html");
    }

    #endregion

    [Test]
    public Task CustomOptions()
    {
        #region CustomOptions

        var settings = new VerifySettings();
        settings.UseExtension("html");
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

        var html = @"<!DOCTYPE html>
<html>
<body>

<h1>My First Heading</h1>
<p>My first paragraph.</p>

</body>
</html>";
        return Verify(html, settings);
    }

    void CustomOptionsGlobal()
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