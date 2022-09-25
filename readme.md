# <img src="/src/icon.png" height="30px"> Verify.AngleSharp

[![Build status](https://ci.appveyor.com/api/projects/status/ff4ms9mevndkui7l?svg=true)](https://ci.appveyor.com/project/SimonCropp/Verify-AngleSharp)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.AngleSharp.svg)](https://www.nuget.org/packages/Verify.AngleSharp/)

Extends [Verify](https://github.com/VerifyTests/Verify) with Html verification utilities via [AngleSharp](https://github.com/AngleSharp/AngleSharp).



## NuGet package

https://nuget.org/packages/Verify.AngleSharp/


## Comparer Usage

Extends [Verify](https://github.com/VerifyTests/Verify) to allow [comparison](https://github.com/VerifyTests/Verify/blob/master/docs/comparer.md) of htm and html files via [AngleSharp](https://github.com/AngleSharp/AngleSharp.Diffing).


### Initialize

Call `VerifyAngleSharpDiffing.Initialize()` once at assembly load time.

Initialize takes an optional `Action<IDiffingStrategyCollection>` to control settings at a global level:

<!-- snippet: Initialize -->
<a id='snippet-initialize'></a>
```cs
[ModuleInitializer]
public static void Init()
{
    VerifyAngleSharpDiffing.Initialize();
```
<sup><a href='/src/Tests/ModuleInitializer.cs#L3-L10' title='Snippet source file'>snippet source</a> | <a href='#snippet-initialize' title='Start of snippet'>anchor</a></sup>
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
<a id='snippet-sample'></a>
```cs
[Test]
public Task Sample()
{
    var html = @"<!DOCTYPE html>
<html>
<body>

<h1>My First Heading</h1>
<p>My first paragraph.</p>

</body>
</html>";
    return Verify(html, "html");
}
```
<sup><a href='/src/Tests/Samples.cs#L109-L126' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample' title='Start of snippet'>anchor</a></sup>
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
<a id='snippet-customoptions'></a>
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
<sup><a href='/src/Tests/Samples.cs#L131-L153' title='Snippet source file'>snippet source</a> | <a href='#snippet-customoptions' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Global settings

<!-- snippet: CustomOptionsGlobal -->
<a id='snippet-customoptionsglobal'></a>
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
<sup><a href='/src/Tests/Samples.cs#L169-L190' title='Snippet source file'>snippet source</a> | <a href='#snippet-customoptionsglobal' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Pretty Print

Html can be pretty printed.

<!-- snippet: PrettyPrintHtml -->
<a id='snippet-prettyprinthtml'></a>
```cs
[Test]
public Task PrettyPrintHtml()
{
    var html = @"<!DOCTYPE html>
<html><body><h1>My First Heading</h1>
<p>My first paragraph.</p></body></html>";
    return Verify(html, "html")
        .PrettyPrintHtml();
}
```
<sup><a href='/src/Tests/Samples.cs#L9-L21' title='Snippet source file'>snippet source</a> | <a href='#snippet-prettyprinthtml' title='Start of snippet'>anchor</a></sup>
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
<a id='snippet-prettyprinthtmlwithnodemanipulation'></a>
```cs
[Test]
public Task PrettyPrintHtmlWithNodeManipulation()
{
    var html = @"<!DOCTYPE html>
<html>
<body>
<h1>My First Heading</h1>
<p>My first paragraph.</p>
</body>
</html>";
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
<sup><a href='/src/Tests/Samples.cs#L84-L107' title='Snippet source file'>snippet source</a> | <a href='#snippet-prettyprinthtmlwithnodemanipulation' title='Start of snippet'>anchor</a></sup>
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
<a id='snippet-scrubemptydivs'></a>
```cs
[Test]
public Task ScrubEmptyDivs()
{
    var html = @"<!DOCTYPE html>
<html>
<body>
<div>My First Heading</div>
<div></div>
</body>
</html>";
    return Verify(html, "html")
        .PrettyPrintHtml(nodes => nodes.ScrubEmptyDivs());
}
```
<sup><a href='/src/Tests/Samples.cs#L23-L39' title='Snippet source file'>snippet source</a> | <a href='#snippet-scrubemptydivs' title='Start of snippet'>anchor</a></sup>
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
<a id='snippet-scrubattributes'></a>
```cs
[Test]
public Task ScrubAttributes()
{
    var html = @"<!DOCTYPE html>
<html>
<body>
<div id='a'>My First Heading</div>
</body>
</html>";
    return Verify(html, "html")
        .PrettyPrintHtml(nodes => nodes.ScrubAttributes("id"));
}
```
<sup><a href='/src/Tests/Samples.cs#L41-L56' title='Snippet source file'>snippet source</a> | <a href='#snippet-scrubattributes' title='Start of snippet'>anchor</a></sup>
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
<a id='snippet-scrubattributewithnewvalue'></a>
```cs
[Test]
public Task ScrubAttributeWithNewValue()
{
    var html = @"<!DOCTYPE html>
<html>
<body>
<div id='a'>My First Heading</div>
</body>
</html>";
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
<sup><a href='/src/Tests/Samples.cs#L58-L82' title='Snippet source file'>snippet source</a> | <a href='#snippet-scrubattributewithnewvalue' title='Start of snippet'>anchor</a></sup>
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
