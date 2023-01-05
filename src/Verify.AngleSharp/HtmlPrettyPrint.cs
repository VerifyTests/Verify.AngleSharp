using AngleSharp.Diffing.Extensions;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace VerifyTests.AngleSharp;

public static class HtmlPrettyPrint
{
    public static void All(Action<INodeList>? action = null)
    {
        VerifierSettings.AddScrubber("html", builder => CleanSource(builder, action));
        VerifierSettings.AddScrubber("htm", builder => CleanSource(builder, action));
    }

    public static void ScrubEmptyDivs(this INodeList nodes) =>
        ScrubEmptyDivs(nodes.OfType<IElement>());

    public static void ScrubEmptyDivs(this IEnumerable<IElement> elements)
    {
        foreach (var element in elements.DescendentsAndSelf<IElement>())
        {
            TryScrubDiv(element);
        }
    }

    public static bool TryScrubDiv(this IElement element)
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

    public static void ScrubAttributes(this INodeList nodes, string name) =>
        ScrubAttributes(nodes.OfType<IElement>(), name);

    public static void ScrubAttributes(this INodeList nodes, Func<IAttr, bool> match) =>
        ScrubAttributes(nodes.OfType<IElement>(), match);

    public static void ScrubAttributes(this INodeList nodes, Func<IAttr, string?> match) =>
        ScrubAttributes(nodes.OfType<IElement>(), match);

    public static void ScrubAttributes(this IEnumerable<IElement> elements, string name) =>
        elements.ScrubAttributes(x => x.Name == name);

    public static void ScrubAttributes(this IEnumerable<IElement> elements, Func<IAttr, string?> tryGetValue)
    {
        foreach (var element in elements.DescendentsAndSelf<IElement>())
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

    public static void ScrubAttributes(this IEnumerable<IElement> elements, Func<IAttr, bool> match)
    {
        foreach (var element in elements.DescendentsAndSelf<IElement>())
        {
            foreach (var attribute in element.Attributes.ToList())
            {
                if (match(attribute))
                {
                    element.RemoveAttribute(attribute.Name);
                }
            }
        }
    }

    public static void PrettyPrintHtml(
        this VerifySettings settings,
        Action<INodeList>? action = null)
    {
        settings.AddScrubber("html", builder => CleanSource(builder, action));
        settings.AddScrubber("htm", builder => CleanSource(builder, action));
    }

    public static SettingsTask PrettyPrintHtml(
        this SettingsTask settings,
        Action<INodeList>? action = null)
    {
        settings.AddScrubber("html", builder => CleanSource(builder, action));
        settings.AddScrubber("htm", builder => CleanSource(builder, action));
        return settings;
    }

    static StringBuilder CleanSource(StringBuilder builder, Action<INodeList>? action)
    {
        var source = builder.ToString();
        var document = Parse(source);
        action?.Invoke(document);

        builder.Clear();
        var formatter = new PrettyMarkupFormatter
        {
            Indentation = "  ",
            NewLine = "\n"
        };
        using var writer = new StringWriter(builder);
        document.ToHtml(writer, formatter);
        writer.Flush();
        return builder;
    }

    static INodeList Parse(string source)
    {
        var parser = new HtmlParser();
        if (source.StartsWith("<!DOCTYPE html>", StringComparison.InvariantCultureIgnoreCase) ||
            source.StartsWith("<html>", StringComparison.InvariantCultureIgnoreCase))
        {
            return parser.ParseDocument(source).ChildNodes;
        }

        var dom = parser.ParseDocument("<html><body></body></html>");
        return parser.ParseFragment(source, dom.Body!);
    }
}