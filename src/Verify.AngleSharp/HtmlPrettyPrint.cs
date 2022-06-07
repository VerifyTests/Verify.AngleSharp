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

    public static void ScrubEmptyDivs(this INodeList nodes)
    {
        foreach (var element in nodes.DescendentsAndSelf<IElement>())
        {
            CleanIfDiv(element);
        }
    }

    static void CleanIfDiv(IElement node)
    {
        if (node is not IHtmlDivElement div)
        {
            return;
        }

        div.InnerHtml = div.InnerHtml.TrimEnd();
        if (node.HasAttributes())
        {
            return;
        }

        if (!node.HasChildNodes)
        {
            node.RemoveFromParent();
            return;
        }

        if (node.Children.Length == 1)
        {
            var child = node.FirstChild;
            node.Parent!.AppendChild(child);
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