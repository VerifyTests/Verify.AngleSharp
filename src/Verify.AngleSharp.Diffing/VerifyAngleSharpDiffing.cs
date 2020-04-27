using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Diffing;
using AngleSharp.Diffing.Strategies;
using Verify;

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
        Task<bool> Func(VerifySettings settings, Stream stream1, Stream stream2) =>
            Compare(settings, stream1, stream2, action);

        SharedVerifySettings.RegisterComparer("html", Func);
        SharedVerifySettings.RegisterComparer("htm", Func);
    }

    static async Task<bool> Compare(
        VerifySettings settings,
        Stream stream1,
        Stream stream2,
        Action<IDiffingStrategyCollection>? action)
    {
        var builder = DiffBuilder.Compare(await stream1.ReadString());
        builder.WithTest(await stream2.ReadString());

        if (action != null)
        {
            builder.WithOptions(action);
        }

        if (settings.GetCompareSettings(out var innerSettings))
        {
            builder.WithOptions(innerSettings.Action);
        }

        var diffs = builder.Build();
        return !diffs.Any();
    }
}