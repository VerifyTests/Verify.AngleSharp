/// <summary>
/// Full scrub over a document of 300 divs, exercising the InnerHtml round trip that the
/// legacy TryScrubDiv performed for every div. ParseOnly is included so the parse cost
/// common to both can be subtracted.
/// </summary>
[MemoryDiagnoser]
public class ScrubEmptyDivsBenchmarks
{
    static readonly HtmlParser parser = new();
    static readonly string html = Build();

    static string Build()
    {
        var builder = new StringBuilder(
            """
            <!DOCTYPE html>
            <html>
            <body>
            """);
        for (var i = 0; i < 100; i++)
        {
            builder.Append(
                $"""
                 <div class="row"><p>paragraph {i}</p><p>a second paragraph carrying some text</p>
                   </div>
                 <div><p>only child {i}</p></div>
                 <div>   </div>

                 """);
        }

        builder.Append(
            """
            </body>
            </html>
            """);
        return builder.ToString();
    }

    [Benchmark(Description = "Parse only (subtract from the two below)")]
    public IDocument ParseOnly() =>
        parser.ParseDocument(html);

    [Benchmark(Baseline = true, Description = "Legacy parse + scrub")]
    public IDocument LegacyParseAndScrub()
    {
        var document = parser.ParseDocument(html);
        Legacy.ScrubEmptyDivs(document.ChildNodes.OfType<IElement>());
        return document;
    }

    [Benchmark(Description = "Current parse + scrub")]
    public IDocument CurrentParseAndScrub()
    {
        var document = parser.ParseDocument(html);
        document.ChildNodes.ScrubEmptyDivs();
        return document;
    }
}
