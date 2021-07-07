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
        public static void All()
        {
            VerifierSettings.AddScrubber("html", Scrubber);
            VerifierSettings.AddScrubber("htm", Scrubber);
        }

        public static void PrettyPrintHtml(
            this VerifySettings settings)
        {
            settings.AddScrubber("html", Scrubber);
            settings.AddScrubber("htm", Scrubber);
        }

        static void Scrubber(StringBuilder builder) => CleanSource(builder);

        public static SettingsTask PrettyPrintHtml(
            this SettingsTask settings)
        {
            settings.AddScrubber("html", Scrubber);
            settings.AddScrubber("htm", Scrubber);
            return settings;
        }

        static StringBuilder CleanSource(StringBuilder builder)
        {
            var source = builder.ToString();
            var parser = new HtmlParser();
            INodeList document;
            if (source.StartsWith("<!DOCTYPE html>", StringComparison.InvariantCultureIgnoreCase) ||
                source.StartsWith("<html>", StringComparison.InvariantCultureIgnoreCase))
            {
                document = parser.ParseFragment(source, null);
            }
            else
            {
                var dom = parser.ParseDocument("<html><body></body></html>");
                document = parser.ParseFragment(source, dom.Body);
            }

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
    }
}