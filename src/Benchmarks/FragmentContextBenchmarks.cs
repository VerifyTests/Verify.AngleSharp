/// <summary>
/// Fragment parsing needs a context element, which Parse obtains by parsing a throwaway
/// document on every call. This measures whether that throwaway is worth avoiding, relative
/// to the fragment parse it supports.
/// </summary>
[MemoryDiagnoser]
public class FragmentContextBenchmarks
{
    static readonly HtmlParser parser = new();

    const string fragment =
        """
        <p>My first paragraph.</p>
        <ul><li>one</li><li>two</li><li>three</li></ul>
        <div class="card"><span>content</span></div>
        """;

    [Benchmark(Baseline = true, Description = "Legacy context document only")]
    public IDocument LegacyContextDocumentOnly() =>
        parser.ParseDocument("<html><body></body></html>");

    [Benchmark(Description = "Current context document only")]
    public IDocument CurrentContextDocumentOnly() =>
        parser.ParseDocument(string.Empty);

    [Benchmark(Description = "Legacy context document + fragment")]
    public INodeList LegacyContextDocumentAndFragment()
    {
        var document = parser.ParseDocument("<html><body></body></html>");
        return parser.ParseFragment(fragment, document.Body!);
    }

    [Benchmark(Description = "Current context document + fragment")]
    public INodeList CurrentContextDocumentAndFragment()
    {
        var document = parser.ParseDocument(string.Empty);
        return parser.ParseFragment(fragment, document.Body!);
    }
}
