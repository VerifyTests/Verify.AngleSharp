using System.Threading.Tasks;
using AngleSharp.Diffing;
using AngleSharp.Diffing.Core;
using Verify;
using VerifyNUnit;
using NUnit.Framework;

[TestFixture]
public class Samples
{
    static Samples()
    {
        #region Initialize
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

    #region Sample
    [Test]
    public Task Sample()
    {
        var settings = new VerifySettings();
        settings.UseExtension("html");
        var html = @"<!DOCTYPE html>
<html>
<body>

<h1>My First Heading</h1>
<p>My first paragraph.</p>

</body>
</html>";
        return Verifier.Verify(html, settings);
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
        return Verifier.Verify(html, settings);
    }
}