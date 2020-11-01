using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Diffing;
using AngleSharp.Diffing.Strategies;

namespace VerifyTests
{
    public static class VerifyAngleSharpDiffing
    {
        public static void AngleSharpDiffingSettings(
            this VerifySettings settings,
            Action<IDiffingStrategyCollection> options)
        {
            Guard.AgainstNull(settings, nameof(settings));
            settings.Context["AngleSharpDiffing"] = new CompareSettings(options);
        }

        static bool GetCompareSettings(
            this IReadOnlyDictionary<string,object> context,
            [NotNullWhen(true)] out CompareSettings? pagesSettings)
        {
            if (context.TryGetValue("AngleSharpDiffing", out var value))
            {
                pagesSettings = (CompareSettings) value;
                return true;
            }

            pagesSettings = null;
            return false;
        }

        public static void Initialize(Action<IDiffingStrategyCollection>? action = null)
        {
            Task<CompareResult> Func(Stream received, Stream verified, IReadOnlyDictionary<string, object> context) =>
                Compare(received, verified, context, action);

            VerifierSettings.RegisterComparer("html", Func);
            VerifierSettings.RegisterComparer("htm", Func);
        }

        static async Task<CompareResult> Compare(
            Stream received,
            Stream verified,
            IReadOnlyDictionary<string, object> context,
            Action<IDiffingStrategyCollection>? action)
        {
            var builder = DiffBuilder.Compare(await verified.ReadString());
            builder.WithTest(await received.ReadString());

            if (action != null)
            {
                builder.WithOptions(action);
            }

            if (context.GetCompareSettings(out var innerSettings))
            {
                builder.WithOptions(innerSettings.Action);
            }

            var diffs = builder.Build().ToList();
            if (diffs.Any())
            {
                var stringBuilder = new StringBuilder(Environment.NewLine);
                foreach (var diff in diffs)
                {
                    DiffConverter.Append(diff, stringBuilder);
                }

                return CompareResult.NotEqual(stringBuilder.ToString());
            }

            return CompareResult.Equal;
        }
    }
}