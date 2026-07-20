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
        var html = """
                   <p>My first paragraph.</p>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml();
    }
}
