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
            Guard.AgainstNull(settings, nameof(settings));
            settings.AddScrubber("html", Scrubber);
            settings.AddScrubber("htm", Scrubber);
        }

        static void Scrubber(StringBuilder builder) => CleanSource(builder);

        public static SettingsTask PrettyPrintHtml(
            this SettingsTask settings)
        {
            Guard.AgainstNull(settings, nameof(settings));
            settings.AddScrubber("html", Scrubber);
            settings.AddScrubber("htm", Scrubber);
            return settings;
        }

        static PrettyMarkupFormatter formatter = new()
        {
            Indentation = "  ",
            NewLine = "\n"
        };

        static StringBuilder CleanSource(StringBuilder builder)
        {
            var source = builder.ToString();
            HtmlParser parser = new();
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
            using StringWriter writer = new(builder);
            document.ToHtml(writer, formatter);
            writer.Flush();
            return builder;
        }
    }
}