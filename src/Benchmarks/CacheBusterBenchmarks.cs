/// <summary>
/// The regex substitution applied to every href and src attribute.
/// WithVersion is a URL carrying a cache buster, WithoutVersion is the far more common case
/// of an ordinary link, which the pattern can never match.
/// </summary>
[MemoryDiagnoser]
public class CacheBusterBenchmarks
{
    const string pattern = @"([^""?]+[?&]v=)[\w\-]+";
    const string replacement = "$1{TAG_HELPER_VERSION}";

    static readonly Regex cached = new(pattern);
    static readonly Regex cachedCompiled = new(pattern, RegexOptions.Compiled);

    [Params(
        "/css/site.css?v=r2K1aJs2_7mdAedOAb0OQXXTwOVHY3K46ElgPZWqeuI",
        "/css/site.css")]
    public string Value { get; set; } = null!;

    [Benchmark(Baseline = true)]
    public string LegacyStaticRegex() =>
        Regex.Replace(Value, pattern, replacement);

    [Benchmark]
    public string CachedRegex() =>
        cached.Replace(Value, replacement);

    [Benchmark]
    public string CachedRegexCompiled() =>
        cachedCompiled.Replace(Value, replacement);

    [Benchmark]
    public string? CurrentCachedWithPreCheck()
    {
        if (Value.IndexOf("v=", StringComparison.Ordinal) == -1)
        {
            return null;
        }

        return cached.Replace(Value, replacement);
    }
}
