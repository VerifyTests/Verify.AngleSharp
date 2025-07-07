[TestFixture]
public class ScrubBrowserLinkTests
{
    [Test]
    public Task ScrubBrowserLinkWithoutWhitespace()
    {
        var html = """
                   <!DOCTYPE html>
                   <html>
                     <head>
                       <meta charset="utf-8">
                     </head>
                     <body>
                       <h1>My Heading</h1>
                       <!-- Visual Studio Browser Link --><script src="/_vs/browserLink"></script><script src="/_framework/aspnetcore-browser-refresh.js"></script><!-- End Browser Link -->
                       </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml(nodes => nodes.ScrubBrowserLink());
    }

    [Test]
    public Task ScrubBrowserLinkWithWhitespace()
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
                       <script src="/_vs/browserLink"></script>

                       <script src="/_framework/aspnetcore-browser-refresh.js"></script>
                       <!-- End Browser Link -->

                       </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml(nodes => nodes.ScrubBrowserLink());
    }

    [Test]
    public Task DoesNotScrubOtherScripts()
    {
        var html = """
                   <!DOCTYPE html>
                   <html>
                     <head>
                       <meta charset="utf-8">
                       <script src="/_vs/browserLink"></script>
                       <script src="/js/site.js"></script>
                       <!-- Some other comment -->
                     </head>
                     <body>
                       <h1>My Heading</h1>
                     </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml(nodes => nodes.ScrubBrowserLink());
    }
}