namespace VerifyTests;

public static class VerifyAngleSharpDiffing
{
    public static void AngleSharpDiffingSettings(
        this VerifySettings settings,
        Action<IDiffingStrategyCollection> options) =>
        settings.Context["AngleSharpDiffing"] =
            new CompareSettings
            {
                Action = options
            };

    public static SettingsTask AngleSharpDiffingSettings(
        this SettingsTask settings,
        Action<IDiffingStrategyCollection> options)
    {
        settings.CurrentSettings.AngleSharpDiffingSettings(options);
        return settings;
    }

    static bool GetCompareSettings(
        this IReadOnlyDictionary<string, object> context,
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

    public static bool Initialized { get; private set; }

    public static void Initialize(Action<IDiffingStrategyCollection>? action = null)
    {
        if (Initialized)
        {
            throw new("Already Initialized");
        }

        Initialized = true;

        InnerVerifier.ThrowIfVerifyHasBeenRun();

        Task<CompareResult> Func(string received, string verified, IReadOnlyDictionary<string, object> context)
        {
            var compare = Compare(received, verified, context, action);
            return Task.FromResult(compare);
        }

        VerifierSettings.RegisterStringComparer("html", Func);
        VerifierSettings.RegisterStringComparer("htm", Func);
    }

    static CompareResult Compare(
        string received,
        string verified,
        IReadOnlyDictionary<string, object> context,
        Action<IDiffingStrategyCollection>? action)
    {
        var builder = DiffBuilder.Compare(verified);
        builder.WithTest(received);

        if (action != null)
        {
            builder.WithOptions(action);
        }

        if (context.GetCompareSettings(out var innerSettings))
        {
            builder.WithOptions(innerSettings.Action);
        }

        var diffs = builder.Build().ToList();
        if (diffs.Count != 0)
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