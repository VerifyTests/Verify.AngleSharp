using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Diffing;
using AngleSharp.Diffing.Strategies;
using Verify;
using CompareResult = Verify.CompareResult;

public static class VerifyAngleSharpDiffing
{
    public static void AngleSharpDiffingSettings(
        this VerifySettings settings,
        Action<IDiffingStrategyCollection> options)
    {
        Guard.AgainstNull(settings, nameof(settings));
        settings.Data["AngleSharpDiffing"] = new CompareSettings(options);
    }

    static bool GetCompareSettings(
        this VerifySettings settings,
        [NotNullWhen(true)] out CompareSettings? pagesSettings)
    {
        Guard.AgainstNull(settings, nameof(settings));
        if (settings.Data.TryGetValue("AngleSharpDiffing", out var value))
        {
            pagesSettings = (CompareSettings) value;
            return true;
        }

        pagesSettings = null;
        return false;
    }

    public static void Initialize(Action<IDiffingStrategyCollection>? action = null)
    {
        Task<CompareResult> Func(VerifySettings settings, Stream received, Stream verified) =>
            Compare(settings, received, verified, action);

        SharedVerifySettings.RegisterComparer("html", Func);
        SharedVerifySettings.RegisterComparer("htm", Func);
    }

    static async Task<CompareResult> Compare(
        VerifySettings settings,
        Stream received,
        Stream verified,
        Action<IDiffingStrategyCollection>? action)
    {
        var builder = DiffBuilder.Compare(await verified.ReadString());
        builder.WithTest(await received.ReadString());

        if (action != null)
        {
            builder.WithOptions(action);
        }

        if (settings.GetCompareSettings(out var innerSettings))
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