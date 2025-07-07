# <img src="/src/icon.png" height="30px"> Verify.AngleSharp

[![Discussions](https://img.shields.io/badge/Verify-Discussions-yellow?svg=true&label=)](https://github.com/orgs/VerifyTests/discussions)
[![Build status](https://ci.appveyor.com/api/projects/status/ff4ms9mevndkui7l?svg=true)](https://ci.appveyor.com/project/SimonCropp/Verify-AngleSharp)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.AngleSharp.svg)](https://www.nuget.org/packages/Verify.AngleSharp/)

Extends [Verify](https://github.com/VerifyTests/Verify) with Html verification utilities via [AngleSharp](https://github.com/AngleSharp/AngleSharp).<!-- singleLineInclude: intro. path: /docs/intro.include.md -->

**See [Milestones](../../milestones?state=closed) for release notes.**


## Sponsors


### Entity Framework Extensions<!-- include: zzz. path: /docs/zzz.include.md -->

[Entity Framework Extensions](https://entityframework-extensions.net/?utm_source=simoncropp&utm_medium=Verify.AngleSharp) is a major sponsor and is proud to contribute to the development this project.

[![Entity Framework Extensions](https://raw.githubusercontent.com/VerifyTests/Verify.AngleSharp/refs/heads/main/docs/zzz.png)](https://entityframework-extensions.net/?utm_source=simoncropp&utm_medium=Verify.AngleSharp)<!-- endInclude -->


## NuGet

 * https://nuget.org/packages/Verify.AngleSharp


## Comparer Usage

Extends [Verify](https://github.com/VerifyTests/Verify) to allow [comparison](https://github.com/VerifyTests/Verify/blob/master/docs/comparer.md) of htm and html files via [AngleSharp](https://github.com/AngleSharp/AngleSharp.Diffing).


### Initialize

Call `VerifyAngleSharpDiffing.Initialize()` once at assembly load time.

Initialize takes an optional `Action<IDiffingStrategyCollection>` to control settings at a global level:

<!-- snippet: Initialize -->
<a id='snippet-Initialize'></a>
```cs
[ModuleInitializer]
public static void Init() =>
    VerifyAngleSharpDiffing.Initialize();
```
<sup><a href='/src/Tests/ModuleInitializer.cs#L3-L9' title='Snippet source file'>snippet source</a> | <a href='#snippet-Initialize' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Verify html

Given an existing verified file:

<!-- snippet: Samples.Sample.verified.html -->
<a id='snippet-Samples.Sample.verified.html'></a>
```html
<!DOCTYPE html>
<html>
<body>
  <h1>My First Heading</h1>
  <p>My first paragraph.</p>
</body>
</html>
```
<sup><a href='/src/Tests/Samples.Sample.verified.html#L1-L7' title='Snippet source file'>snippet source</a> | <a href='#snippet-Samples.Sample.verified.html' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

And a test:

<!-- snippet: Sample -->
<a id='snippet-Sample'></a>
```cs
[Test]
public Task Sample()
{
    var html =
        """
        <!DOCTYPE html>
        <html>
          <body>
            <h1>My First Heading</h1>
            <p>My first paragraph.</p>
          </body>
        </html>
        """;
    return Verify(html, "html");
}
```
<sup><a href='/src/Tests/Samples.cs#L196-L214' title='Snippet source file'>snippet source</a> | <a href='#snippet-Sample' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Note that the input html differs from the verified html, but not in a semantically significant way. Hence this test will pass.


### Diff results

If the comparison fails, the resulting differences will be included in the test result displayed to the user.

For example if, in the above html, `<h1>My First Heading</h1>` changes to `<h1>First Heading</h1>` then the following will be printed in the test results:

```
Comparer result:
 * Node Diff
   Path: h1(0) > #text(0)
   Received: First Heading
   Verified: My First Heading
```


### Test level settings

Settings can also be controlled for a specific test.

<!-- snippet: CustomOptions -->
<a id='snippet-CustomOptions'></a>
```cs
var settings = new VerifySettings();
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
```
<sup><a href='/src/Tests/Samples.cs#L244-L266' title='Snippet source file'>snippet source</a> | <a href='#snippet-CustomOptions' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Global settings

<!-- snippet: CustomOptionsGlobal -->
<a id='snippet-CustomOptionsGlobal'></a>
```cs
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
```
<sup><a href='/src/Tests/Samples.cs#L282-L302' title='Snippet source file'>snippet source</a> | <a href='#snippet-CustomOptionsGlobal' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Verify svg

Given an existing verified file:

<!-- snippet: Samples.SvgSample.verified.svg -->
<a id='snippet-Samples.SvgSample.verified.svg'></a>
```svg
<svg version="1.1" xmlns="http://www.w3.org/2000/svg"
     width="300" height="200">
  <rect fill="red" width="100%" height="100%" />
  <circle fill="green" cx="150" cy="100" r="80" />
  <text fill="white"
        x="150" y="125"
        font-size="60"
        text-anchor="middle">
  SVG
  </text>
</svg>
```
<sup><a href='/src/Tests/Samples.SvgSample.verified.svg#L1-L11' title='Snippet source file'>snippet source</a> | <a href='#snippet-Samples.SvgSample.verified.svg' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

And a test:

<!-- snippet: SvgSample -->
<a id='snippet-SvgSample'></a>
```cs
[Test]
public Task SvgSample()
{
    var svg =
        """
        <svg version="1.1"
             width="300" height="200"
             xmlns="http://www.w3.org/2000/svg">
          <rect width="100%" height="100%" fill="red" />
          <circle cx="150" cy="100" r="80" fill="green" />
          <text x="150" y="125"
                font-size="60"
                text-anchor="middle"
                fill="white">
          SVG
          </text>
        </svg>
        """;
    return Verify(svg, "svg");
}
```
<sup><a href='/src/Tests/Samples.cs#L216-L239' title='Snippet source file'>snippet source</a> | <a href='#snippet-SvgSample' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Note that the input svg differs from the verified svg, but not in a semantically significant way. Hence this test will pass.


## Pretty Print

Html can be pretty printed.

<!-- snippet: PrettyPrintHtml -->
<a id='snippet-PrettyPrintHtml'></a>
```cs
[Test]
public Task PrettyPrintHtml()
{
    var html = """
               <!DOCTYPE html>
               <html><body><h1>My First Heading</h1>
               <p>My first paragraph.</p></body></html>
               """;
    return Verify(html, "html")
        .PrettyPrintHtml();
}
```
<sup><a href='/src/Tests/Samples.cs#L4-L18' title='Snippet source file'>snippet source</a> | <a href='#snippet-PrettyPrintHtml' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Results in 

<!-- snippet: Samples.PrettyPrintHtml.verified.html -->
<a id='snippet-Samples.PrettyPrintHtml.verified.html'></a>
```html
<!DOCTYPE html>
<html>
  <head></head>
  <body>
    <h1>My First Heading</h1>
    <p>My first paragraph.</p>
  </body>
</html>
```
<sup><a href='/src/Tests/Samples.PrettyPrintHtml.verified.html#L1-L8' title='Snippet source file'>snippet source</a> | <a href='#snippet-Samples.PrettyPrintHtml.verified.html' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

To apply this to all `html` files use `HtmlPrettyPrint.All();`


### Manipulate Html

Nodes can be manipulated as part of the pretty print:

<!-- snippet: PrettyPrintHtmlWithNodeManipulation -->
<a id='snippet-PrettyPrintHtmlWithNodeManipulation'></a>
```cs
[Test]
public Task PrettyPrintHtmlWithNodeManipulation()
{
    var html = """
               <!DOCTYPE html>
               <html>
                 <body>
                   <h1>My First Heading</h1>
                   <p>My first paragraph.</p>
                 </body>
               </html>
               """;
    return Verify(html, "html")
        .PrettyPrintHtml(
            nodes =>
            {
                foreach (var node in nodes.QuerySelectorAll("h1"))
                {
                    node.Remove();
                }
            });
}
```
<sup><a href='/src/Tests/Samples.cs#L144-L169' title='Snippet source file'>snippet source</a> | <a href='#snippet-PrettyPrintHtmlWithNodeManipulation' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Results in 

<!-- snippet: Samples.PrettyPrintHtmlWithNodeManipulation.verified.html -->
<a id='snippet-Samples.PrettyPrintHtmlWithNodeManipulation.verified.html'></a>
```html
<!DOCTYPE html>
<html>
  <head></head>
  <body>
    <p>My first paragraph.</p>
  </body>
</html>
```
<sup><a href='/src/Tests/Samples.PrettyPrintHtmlWithNodeManipulation.verified.html#L1-L7' title='Snippet source file'>snippet source</a> | <a href='#snippet-Samples.PrettyPrintHtmlWithNodeManipulation.verified.html' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## AngleSharp helpers


### ScrubEmptyDivs

<!-- snippet: ScrubEmptyDivs -->
<a id='snippet-ScrubEmptyDivs'></a>
```cs
[Test]
public Task ScrubEmptyDivs()
{
    var html = """
               <!DOCTYPE html>
               <html>
                 <body>
                   <div>My First Heading</div>
                   <div></div>
                 </body>
               </html>
               """;
    return Verify(html, "html")
        .PrettyPrintHtml(nodes => nodes.ScrubEmptyDivs());
}
```
<sup><a href='/src/Tests/Samples.cs#L20-L38' title='Snippet source file'>snippet source</a> | <a href='#snippet-ScrubEmptyDivs' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Results in:

<!-- snippet: Samples.ScrubEmptyDivs.verified.html -->
<a id='snippet-Samples.ScrubEmptyDivs.verified.html'></a>
```html
<!DOCTYPE html>
<html>
  <head></head>
  <body>
    <div>My First Heading</div>
  </body>
</html>
```
<sup><a href='/src/Tests/Samples.ScrubEmptyDivs.verified.html#L1-L7' title='Snippet source file'>snippet source</a> | <a href='#snippet-Samples.ScrubEmptyDivs.verified.html' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### ScrubAttributes


#### Removal

<!-- snippet: ScrubAttributes -->
<a id='snippet-ScrubAttributes'></a>
```cs
[Test]
public Task ScrubAttributes()
{
    var html = """
               <!DOCTYPE html>
               <html>
                 <body>
                   <div id='a'>My First Heading</div>
                 </body>
               </html>
               """;
    return Verify(html, "html")
        .PrettyPrintHtml(nodes => nodes.ScrubAttributes("id"));
}
```
<sup><a href='/src/Tests/Samples.cs#L40-L57' title='Snippet source file'>snippet source</a> | <a href='#snippet-ScrubAttributes' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Results in:

<!-- snippet: Samples.ScrubAttributes.verified.html -->
<a id='snippet-Samples.ScrubAttributes.verified.html'></a>
```html
<!DOCTYPE html>
<html>
  <head></head>
  <body>
    <div>My First Heading</div>
  </body>
</html>
```
<sup><a href='/src/Tests/Samples.ScrubAttributes.verified.html#L1-L7' title='Snippet source file'>snippet source</a> | <a href='#snippet-Samples.ScrubAttributes.verified.html' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Replace Value

<!-- snippet: ScrubAttributeWithNewValue -->
<a id='snippet-ScrubAttributeWithNewValue'></a>
```cs
[Test]
public Task ScrubAttributeWithNewValue()
{
    var html = """
               <!DOCTYPE html>
               <html>
                 <body>
                   <div id='a'>My First Heading</div>
                 </body>
               </html>
               """;
    return Verify(html, "html")
        .PrettyPrintHtml(
            nodes => nodes.ScrubAttributes(x =>
            {
                if (x.Name == "id")
                {
                    return "new value";
                }

                return null;
            }));
}
```
<sup><a href='/src/Tests/Samples.cs#L59-L85' title='Snippet source file'>snippet source</a> | <a href='#snippet-ScrubAttributeWithNewValue' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Results in:

<!-- snippet: Samples.ScrubAttributeWithNewValue.verified.html -->
<a id='snippet-Samples.ScrubAttributeWithNewValue.verified.html'></a>
```html
<!DOCTYPE html>
<html>
  <head></head>
  <body>
    <div id="new value">My First Heading</div>
  </body>
</html>
```
<sup><a href='/src/Tests/Samples.ScrubAttributeWithNewValue.verified.html#L1-L7' title='Snippet source file'>snippet source</a> | <a href='#snippet-Samples.ScrubAttributeWithNewValue.verified.html' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### ScrubAspCacheBusterTagHelper

<!-- snippet: ScrubAspCacheBusterTagHelper -->
<a id='snippet-ScrubAspCacheBusterTagHelper'></a>
```cs
[Test]
public Task ScrubAspCacheBusterTagHelper()
{
    var html = """
               <!DOCTYPE html>
               <html>
                 <head>
                   <link
                    href="/css/site.css?v=r2K1aJs2_7mdAedOAb0OQw3K46ElgPZWqeuI"
                    rel="stylesheet"
                    />
                 </head>
                 <body>
                   <h1>My Heading</h1>
                 </body>
               </html>
               """;
    return Verify(html, "html")
        .PrettyPrintHtml(nodes => nodes.ScrubAspCacheBusterTagHelper());
}
```
<sup><a href='/src/Tests/Samples.cs#L87-L110' title='Snippet source file'>snippet source</a> | <a href='#snippet-ScrubAspCacheBusterTagHelper' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Results in:

<!-- snippet: Samples.ScrubAspCacheBusterTagHelper.verified.html -->
<a id='snippet-Samples.ScrubAspCacheBusterTagHelper.verified.html'></a>
```html
<!DOCTYPE html>
<html>
  <head>
    <link href="/css/site.css?v={TAG_HELPER_VERSION}" rel="stylesheet">
  </head>
  <body>
    <h1>My Heading</h1>
  </body>
</html>
```
<sup><a href='/src/Tests/Samples.ScrubAspCacheBusterTagHelper.verified.html#L1-L9' title='Snippet source file'>snippet source</a> | <a href='#snippet-Samples.ScrubAspCacheBusterTagHelper.verified.html' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### ScrubBrowserLink

<!-- snippet: ScrubBrowserLink -->
<a id='snippet-ScrubBrowserLink'></a>
```cs
[Test]
public Task ScrubBrowserLink()
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
                   <script src="/_vs/browserLink">
                   </script>

                   <script src="/_framework/aspnetcore-browser-refresh.js">
                   </script>
                   <!-- End Browser Link -->

                   </body>
               </html>
               """;
    return Verify(html, "html")
        .PrettyPrintHtml(nodes => nodes.ScrubBrowserLink());
}
```
<sup><a href='/src/Tests/Samples.cs#L112-L142' title='Snippet source file'>snippet source</a> | <a href='#snippet-ScrubBrowserLink' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Results in:

<!-- snippet: Samples.ScrubBrowserLink.verified.html -->
<a id='snippet-Samples.ScrubBrowserLink.verified.html'></a>
```html
<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8">
  </head>
  <body>
    <h1>My Heading</h1>
  </body>
</html>
```
<sup><a href='/src/Tests/Samples.ScrubBrowserLink.verified.html#L1-L9' title='Snippet source file'>snippet source</a> | <a href='#snippet-Samples.ScrubBrowserLink.verified.html' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
