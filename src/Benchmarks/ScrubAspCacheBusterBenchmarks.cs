/// <summary>
/// Full cache buster scrub over a page with many attributes, only a few of which are href or
/// src carrying a version. Legacy reassigned every attribute in the document and ran the
/// static Regex.Replace overload for every href and src.
/// </summary>
[MemoryDiagnoser]
public class ScrubAspCacheBusterBenchmarks
{
    static readonly HtmlParser parser = new();
    static readonly string html = Build();

    static string Build()
    {
        var builder = new StringBuilder("<!DOCTYPE html><html><head>");
        builder.Append("""<link href="/css/site.css?v=r2K1aJs2_7mdAedOAb0OQXXTwOVHY3K46ElgPZWqeuI" rel="stylesheet">""");
        builder.Append("""<script src="/js/site.js?v=4q1jwHbih5xTTZoI1K3CyVNBn6G5cyGXls93d_7XUPA"></script>""");
        builder.Append("</head><body>");
        for (var i = 0; i < 100; i++)
        {
            builder.Append("<div class=\"card shadow\" id=\"card-")
                .Append(i)
                .Append("\" data-index=\"")
                .Append(i)
                .Append("\" role=\"listitem\">")
                .Append("<a href=\"/products/")
                .Append(i)
                .Append("\" title=\"Product\">Product</a>")
                .Append("<img src=\"/images/product-")
                .Append(i)
                .Append(".png\" alt=\"Product image\" loading=\"lazy\">")
                .Append("</div>");
        }

        builder.Append("</body></html>");
        return builder.ToString();
    }

    [Benchmark(Description = "Parse only (subtract from the two below)")]
    public IDocument ParseOnly() =>
        parser.ParseDocument(html);

    [Benchmark(Baseline = true, Description = "Legacy parse + scrub")]
    public IDocument LegacyParseAndScrub()
    {
        var document = parser.ParseDocument(html);
        Legacy.ScrubAspCacheBusterTagHelper(document.ChildNodes.OfType<IElement>());
        return document;
    }

    [Benchmark(Description = "Current parse + scrub")]
    public IDocument CurrentParseAndScrub()
    {
        var document = parser.ParseDocument(html);
        document.ChildNodes.ScrubAspCacheBusterTagHelper();
        return document;
    }
}
