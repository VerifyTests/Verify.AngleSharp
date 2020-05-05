# <img src="/src/icon.png" height="30px"> Verify.AngleSharp.Diffing

[![Build status](https://ci.appveyor.com/api/projects/status/ff4ms9mevndkui7l?svg=true)](https://ci.appveyor.com/project/VerifyTests/Verify-AngleSharp-Diffing)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.AngleSharp.Diffing.svg)](https://www.nuget.org/packages/Verify.AngleSharp.Diffing/)

Extends [Verify](https://github.com/VerifyTests/Verify) to allow [comparison](https://github.com/VerifyTests/Verify/blob/master/docs/comparer.md) of htm and html files via [AngleSharp.Diffing](https://github.com/AngleSharp/AngleSharp.Diffing).

Support is available via a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-verify.anglesharp.diffing?utm_source=nuget-verify.anglesharp.diffing&utm_medium=referral&utm_campaign=enterprise).

toc


## NuGet package

https://nuget.org/packages/Verify.AngleSharp.Diffing/


## Usage


### Initialize

Call `VerifyAngleSharpDiffing.Initialize()` once at assembly load time.

Initialize takes an optional `Action<IDiffingStrategyCollection>` to control settings at a global level:

snippet: Initialize


### Verify html

Given an existing verified file:

snippet: Samples.Sample.verified.html

And a test:

snippet: Sample

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

snippet: CustomOptions


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.