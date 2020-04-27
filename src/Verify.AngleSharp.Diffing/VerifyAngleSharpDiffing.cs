using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Diffing;
using AngleSharp.Diffing.Strategies;
using Verify;

public static class VerifyAngleSharpDiffing
{
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
        var diffs = builder.Build().ToList();
        return !diffs.Any();
    }
}