# <img src="/src/icon.png" height="30px"> Verify.AngleSharp.Diffing

[![Build status](https://ci.appveyor.com/api/projects/status/ff4ms9mevndkui7l?svg=true)](https://ci.appveyor.com/project/SimonCropp/Verify-AngleSharp-Diffing)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.AngleSharp.Diffing.svg)](https://www.nuget.org/packages/Verify.AngleSharp.Diffing/)

Extends [Verify](https://github.com/SimonCropp/Verify) to allow [comparison](https://github.com/SimonCropp/Verify/blob/master/docs/comparer.md) of htm and html files via [AngleSharp.Diffing](https://github.com/AngleSharp/AngleSharp.Diffing).

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

Note that the input html differs from the verified html, but not in a semanticaly significant way. Hence this test will pass.


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.