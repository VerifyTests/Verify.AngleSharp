using System.IO;
using System.Text;
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
            HtmlParser parser = new();
            var document = parser.ParseFragment(builder.ToString(), null);
            builder.Clear();
            using StringWriter writer = new(builder);
            document.ToHtml(writer, formatter);
            writer.Flush();
            return builder;
        }
    }
}