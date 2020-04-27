using System.IO;
using System.Threading.Tasks;
using AngleSharp.Diffing;
using AngleSharp.Diffing.Core;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class Samples :
    VerifyBase
{
    public Samples(ITestOutputHelper output) :
        base(output)
    {
    }

    static Samples()
    {
        #region Initialize
        VerifyAngleSharpDiffing.Initialize(action =>
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
    [Fact]
    public async Task Sample()
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
        await Verify(html, settings);
    }

    #endregion
}