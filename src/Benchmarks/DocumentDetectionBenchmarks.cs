/// <summary>
/// Prefix check that decides whether the source is parsed as a document or a body fragment.
/// Current is a copy of the private HtmlPrettyPrint.IsDocument.
/// </summary>
[MemoryDiagnoser]
public class DocumentDetectionBenchmarks
{
    const StringComparison comparer = StringComparison.OrdinalIgnoreCase;

    const string source =
        """
        <!DOCTYPE html>
        <html lang="en">
          <head><meta charset="utf-8"></head>
          <body><h1>My Heading</h1></body>
        </html>
        """;

    [Benchmark(Baseline = true)]
    public bool LegacyInvariantCulture() =>
        Legacy.IsDocumentInvariantCulture(source);

    [Benchmark]
    public bool LegacyOrdinal() =>
        Legacy.IsDocumentOrdinal(source);

    [Benchmark]
    public bool Current() =>
        IsDocument(source);

    static bool IsDocument(string source)
    {
        var index = 0;
        while (index < source.Length &&
               char.IsWhiteSpace(source[index]))
        {
            index++;
        }

        if (Matches("<!doctype"))
        {
            return true;
        }

        if (!Matches("<html"))
        {
            return false;
        }

        var next = index + "<html".Length;
        return next >= source.Length ||
               source[next] is '>' or '/' ||
               char.IsWhiteSpace(source[next]);

        bool Matches(string tag) =>
            index + tag.Length <= source.Length &&
            string.Compare(source, index, tag, 0, tag.Length, comparer) == 0;
    }
}
