[TestFixture]
public class ScrubAspCacheBusterTagHelperTests
{
    [Test]
    public Task ScrubHrefAttribute()
    {
        var html = """
                   <!DOCTYPE html>
                   <html>
                     <head>
                       <link href="/css/site.css?v=r2K1aJs2_7mdAedOAb0OQXXTwOVHY3K46ElgPZWqeuI" rel="stylesheet" />
                     </head>
                     <body>
                       <h1>My Heading</h1>
                     </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml(nodes => nodes.ScrubAspCacheBusterTagHelper());
    }

    [Test]
    public Task ScrubSrcAttribute()
    {
        var html = """
                   <!DOCTYPE html>
                   <html>
                     <head>
                       <script src="/js/site.js?v=4q1jwHbih5xTTZoI1K3CyVNBn6G5cyGXls93d_7XUPA"></script>
                     </head>
                     <body>
                       <h1>My Heading</h1>
                     </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml(nodes => nodes.ScrubAspCacheBusterTagHelper());
    }

    [Test]
    public Task ScrubImageSrc()
    {
        var html = """
                   <!DOCTYPE html>
                   <html>
                     <body>
                       <img src="/images/logo.png?v=ABcd123" alt="Logo" />
                     </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml(nodes => nodes.ScrubAspCacheBusterTagHelper());
    }

    [Test]
    public Task ScrubWithAmpersandVersionParam()
    {
        var html = """
                   <!DOCTYPE html>
                   <html>
                     <head>
                       <link href="/css/site.css?theme=dark&v=r2K1aJs2_7md" rel="stylesheet" />
                     </head>
                     <body>
                       <h1>My Heading</h1>
                     </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml(nodes => nodes.ScrubAspCacheBusterTagHelper());
    }

    [Test]
    public Task ScrubWithExtraParams()
    {
        var html = """
                   <!DOCTYPE html>
                   <html>
                     <head>
                       <link href="/css/site.css?v=r2K1aJs2_7mm&theme=dark" rel="stylesheet" />
                     </head>
                     <body>
                       <h1>My Heading</h1>
                     </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml(nodes => nodes.ScrubAspCacheBusterTagHelper());
    }

    [Test]
    public Task ScrubMultipleElements()
    {
        var html = """
                   <!DOCTYPE html>
                   <html>
                     <head>
                       <link href="/css/site.css?v=r2K1aJs2" rel="stylesheet" />
                       <script src="/js/site.js?v=4q1jwHbih5-2"></script>
                     </head>
                     <body>
                       <img src="/images/logo.png?v=ABcd123" alt="Logo" />
                       <a href="/about?v=XYZ-123">About</a>
                     </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml(nodes => nodes.ScrubAspCacheBusterTagHelper());
    }

    [Test]
    public Task DoesNotAffectOtherAttributes()
    {
        var html = """
                   <!DOCTYPE html>
                   <html>
                     <head>
                       <link href="/css/site.css?theme=dark" rel="stylesheet" />
                       <script src="/js/site.js?debug=true"></script>
                       <link href="/css/site.css?v=r2K1aJs2" rel="stylesheet" data-version="1.0" />
                     </head>
                     <body>
                       <div class="container" id="main-content">
                         <h1>My Heading</h1>
                       </div>
                       <img src="/images/logo.png?width=200" alt="Logo" />
                       <a href="/about?id=123">About</a>
                     </body>
                   </html>
                   """;
        return Verify(html, "html")
            .PrettyPrintHtml(nodes => nodes.ScrubAspCacheBusterTagHelper());
    }
}