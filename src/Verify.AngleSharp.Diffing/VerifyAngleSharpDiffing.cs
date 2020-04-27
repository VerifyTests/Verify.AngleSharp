using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Diffing;
using AngleSharp.Diffing.Core;
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
                if(diff is NodeDiff nodeDiff)
                {
                    var receivedSource = nodeDiff.Test;
                    var verifiedSource = nodeDiff.Control;
                    stringBuilder.AppendLine($@" * Node Diff
   Path: {receivedSource.Path}
   Received Value: {receivedSource.Node.NodeValue}
   Verified Value: {verifiedSource.Node.NodeValue}");
                    continue;
                }

                if(diff is AttrDiff attrDiff)
                {
                    var receivedSource = attrDiff.Test;
                    var verifiedSource = attrDiff.Control;
                    stringBuilder.AppendLine($@" * Attribute Diff
   Path: {receivedSource.Path}
   Name: {receivedSource.Attribute.Name}
   Received Value: {receivedSource.Attribute.Value}
   Verified Value: {verifiedSource.Attribute.Value}");
                    continue;
                }

                if(diff is UnexpectedAttrDiff unexpectedAttrDiff)
                {
                    var receivedSource = unexpectedAttrDiff.Test;
                    stringBuilder.AppendLine($@" * Unexpected Attribute
   Path: {receivedSource.Path}
   Name: {receivedSource.Attribute.Name}
   Value: {receivedSource.Attribute.Value}");
                    continue;
                }

                if(diff is UnexpectedNodeDiff unexpectedNodeDiff)
                {
                    var receivedSource = unexpectedNodeDiff.Test;
                    stringBuilder.AppendLine($@"* Unexpected Node
   Path: {receivedSource.Path}
   Name: {receivedSource.Node.NodeName}
   Value: {receivedSource.Node.NodeValue}");
                    continue;
                }

                if(diff is MissingAttrDiff missingAttrDiff)
                {
                    var verifiedSource = missingAttrDiff.Control;
                    stringBuilder.AppendLine($@" * Missing Attribute
   Path: {verifiedSource.Path}
   Name: {verifiedSource.Attribute.Name}
   Value: {verifiedSource.Attribute.Value}");
                    continue;
                }

                if(diff is MissingNodeDiff missingNodeDiff)
                {
                    var receivedSource = missingNodeDiff.Control;
                    stringBuilder.AppendLine($@" * Missing Node
   Path: {receivedSource.Path}
   Name: {receivedSource.Node.NodeName}
   Value: {receivedSource.Node.NodeValue}");
                    continue;
                }

                throw new Exception($"Unknown diff: {diff.GetType()}");
            }

            return CompareResult.NotEqual(stringBuilder.ToString());
        }

        return CompareResult.Equal;
    }
}