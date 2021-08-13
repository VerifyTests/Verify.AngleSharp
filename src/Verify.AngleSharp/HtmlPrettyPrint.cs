using System;
using System.IO;
using System.Text;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Parser;
using VerifyTests;

namespace Verify.AngleSharp
{
    public static class HtmlPrettyPrint
    {
        public static void All(Action<INodeList>? action = null)
        {
            VerifierSettings.AddScrubber("html", builder => CleanSource(builder, action));
            VerifierSettings.AddScrubber("htm", builder => CleanSource(builder, action));
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
}