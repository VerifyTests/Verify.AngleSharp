/// <summary>
/// Verbatim copies of the implementations as they were before the fixes, kept only so the
/// benchmarks can measure an actual before and after. Nothing here should be used for real.
/// </summary>
public static class Legacy
{
    const StringComparison comparer = StringComparison.OrdinalIgnoreCase;

    public static bool IsDocumentInvariantCulture(string source) =>
        source.StartsWith("<!DOCTYPE html>", StringComparison.InvariantCultureIgnoreCase) ||
        source.StartsWith("<html>", StringComparison.InvariantCultureIgnoreCase);

    public static bool IsDocumentOrdinal(string source) =>
        source.StartsWith("<!DOCTYPE html>", comparer) ||
        source.StartsWith("<html>", comparer);

    public static void ScrubEmptyDivs(IEnumerable<IElement> elements)
    {
        foreach (var element in elements.DescendantsAndSelf<IElement>())
        {
            TryScrubDiv(element);
        }
    }

    public static bool TryScrubDiv(IElement element)
    {
        if (element is not IHtmlDivElement div)
        {
            return false;
        }

        div.InnerHtml = div.InnerHtml.TrimEnd();
        if (element.HasAttributes())
        {
            return false;
        }

        if (!element.HasChildNodes)
        {
            element.RemoveFromParent();
        }
        else if (element.Children.Length == 1)
        {
            var child = element.FirstChild;
            element.Parent!.AppendChild(child);
        }

        return true;
    }

    public static void ScrubAspCacheBusterTagHelper(IEnumerable<IElement> elements) =>
        ScrubAttributes(
            elements,
            static attr =>
            {
                if (attr.Name.Equals("href", comparer) || attr.Name.Equals("src", comparer))
                {
                    const string pattern = @"([^""?]+[?&]v=)[\w\-]+";
                    const string replacement = "$1{TAG_HELPER_VERSION}";

                    return Regex.Replace(attr.Value, pattern, replacement);
                }

                return attr.Value;
            });

    public static void ScrubAttributes(IEnumerable<IElement> elements, Func<IAttr, string?> tryGetValue)
    {
        foreach (var element in elements.DescendantsAndSelf<IElement>())
        {
            foreach (var attribute in element.Attributes)
            {
                var value = tryGetValue(attribute);
                if (value != null)
                {
                    attribute.Value = value;
                }
            }
        }
    }
}
