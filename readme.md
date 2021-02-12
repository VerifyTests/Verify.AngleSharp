# <img src="/src/icon.png" height="30px"> Verify.AngleSharp

[![Build status](https://ci.appveyor.com/api/projects/status/ff4ms9mevndkui7l?svg=true)](https://ci.appveyor.com/project/SimonCropp/Verify-AngleSharp)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.AngleSharp.svg)](https://www.nuget.org/packages/Verify.AngleSharp/)

Extends [Verify](https://github.com/VerifyTests/Verify) with Html verification utilities via [AngleSharp](https://github.com/AngleSharp/AngleSharp).

Support is available via a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-verify?utm_source=nuget-verify&utm_medium=referral&utm_campaign=enterprise).

<a href='https://dotnetfoundation.org' alt='Part of the .NET Foundation'><img src='https://raw.githubusercontent.com/VerifyTests/Verify/master/docs/dotNetFoundation.svg' height='30px'></a><br>
Part of the <a href='https://dotnetfoundation.org' alt=''>.NET Foundation</a>

<!-- toc -->
## Contents

  * [Comparer Usage](#comparer-usage)
    * [Initialize](#initialize)
    * [Verify html](#verify-html)
    * [Diff results](#diff-results)
    * [Test level settings](#test-level-settings)
    * [PrettyPrin](#prettyprin)
  * [Security contact information](#security-contact-information)<!-- endToc -->


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
<sup><a href='/src/Tests/Samples.cs#L14-L33' title='Snippet source file'>snippet source</a> | <a href='#snippet-initialize' title='Start of snippet'>anchor</a></sup>
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
    return Verifier.Verify(html)
        .UseExtension("html");
}
```
<sup><a href='/src/Tests/Samples.cs#L50-L67' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample' title='Start of snippet'>anchor</a></sup>
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
VerifySettings settings = new();
settings.UseExtension("html");
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
<sup><a href='/src/Tests/Samples.cs#L72-L93' title='Snippet source file'>snippet source</a> | <a href='#snippet-customoptions' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### PrettyPrin

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
    return Verifier.Verify(html)
        .UseExtension("html")
        .PrettyPrintHtml();
}
```
<sup><a href='/src/Tests/Samples.cs#L36-L48' title='Snippet source file'>snippet source</a> | <a href='#snippet-prettyprinthtml' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Results in 

<!-- snippet: Samples.PrettyPrintHtml.verified.html -->
<a id='snippet-Samples.PrettyPrintHtml.verified.html'></a>
```html
<!DOCTYPE html>
<html>
<body>

<h1>My First Heading</h1>
<p>My first paragraph.</p>

</body>
</html>
```
<sup><a href='/src/Tests/Samples.PrettyPrintHtml.verified.html#L1-L9' title='Snippet source file'>snippet source</a> | <a href='#snippet-Samples.PrettyPrintHtml.verified.html' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.
