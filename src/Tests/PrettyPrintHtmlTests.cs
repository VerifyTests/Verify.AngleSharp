[TestFixture]
public class PrettyPrintHtmlTests
{
    [Test]
    public Task DocumentWithHtmlAttributes()
    {
        var html = """
                   <html lang="en">
                     <body>
                       <h1>My Heading</h1>
                     </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml();
    }

    [Test]
    public Task DocumentWithLeadingWhitespace()
    {
        var html = "\n  " +
                   """
                   <!DOCTYPE html>
                   <html>
                     <body>
                       <h1>My Heading</h1>
                     </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml();
    }

    [Test]
    public Task DocumentWithLegacyDoctype()
    {
        var html = """
                   <!DOCTYPE html SYSTEM "about:legacy-compat">
                   <html>
                     <body>
                       <h1>My Heading</h1>
                     </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml();
    }

    [Test]
    public Task FragmentIsNotWrappedInDocument()
    {
        var html = "<p>My first paragraph.</p>";
        return Verify(html, "html")
            .PrettyPrintHtml();
    }

    // Indenting the content of a pre rewrites what it says. The formatter is told to leave the
    // elements whose whitespace is significant alone, so the text survives the round trip.
    [Test]
    public Task PreservesPreformattedText()
    {
        var html = """
                   <div><pre>SELECT [e].[Name]
                   FROM [Employees] AS [e]
                   WHERE [e].[Active] = 1</pre></div>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml();
    }

    [Test]
    public Task PreservesTextareaAndScript()
    {
        var html = """
                   <div><textarea>line one
                   line two</textarea><script>
                   if (x) {
                     go();
                   }
                   </script></div>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml();
    }

    // A table fragment is what an innerHTML capture of a table yields. Parsed against body — where a
    // table tag is not recognised — the tags would be dropped and only the cell text would survive.
    [Test]
    public Task TableSectionFragment()
    {
        var html = "<thead><tr><th>Name</th></tr></thead><tbody><tr><td>Aaron</td></tr></tbody>";
        return Verify(html, "html")
            .PrettyPrintHtml();
    }

    [Test]
    public Task TableRowFragment()
    {
        var html = "<tr><td>Aaron</td><td>FullTime</td></tr>";
        return Verify(html, "html")
            .PrettyPrintHtml();
    }

    [Test]
    public Task TableCellFragment()
    {
        var html = "<td>Aaron</td><td>FullTime</td>";
        return Verify(html, "html")
            .PrettyPrintHtml();
    }

    // A fragment captured from a rendered page routinely leads with a comment — Blazor marks every
    // component boundary with one — and the tag after it still decides how the fragment parses.
    [Test]
    public Task TableFragmentBehindAComment()
    {
        var html = "<!--marker--><thead><tr><th>Name</th></tr></thead>";
        return Verify(html, "html")
            .PrettyPrintHtml();
    }

    // The leading tag decides the parse context, so one that needs no special context still parses
    // against body — including a fragment that opens with text rather than a tag.
    [Test]
    public Task FragmentOpeningWithText()
    {
        var html = "Some text <b>then an element</b>";
        return Verify(html, "html")
            .PrettyPrintHtml();
    }

    // A fragment is parsed against a context element that is never written, and the formatter breaks
    // the line before every node inside an element. So the first node took a break with nothing in
    // front of it and the markup opened on the second line.
    //
    // Compared as text rather than by the registered html comparer: that one diffs the two DOMs, where
    // a blank line before the first node is whitespace between nodes and no difference at all. Every
    // other test here is blind to the thing this one is for.
    [Test]
    public Task FragmentDoesNotOpenWithABlankLine()
    {
        var html = "<p>My first paragraph.</p>";
        return Verify(html, "html")
            .PrettyPrintHtml()
            .UseStringComparer(CompareText);
    }

    static Task<VerifyTests.CompareResult> CompareText(
        string received,
        string verified,
        IReadOnlyDictionary<string, object> context)
    {
        if (string.Equals(received, verified, StringComparison.Ordinal))
        {
            return Task.FromResult(VerifyTests.CompareResult.Equal);
        }

        return Task.FromResult(
            VerifyTests.CompareResult.NotEqual($"Received:{Environment.NewLine}{received}"));
    }
}
