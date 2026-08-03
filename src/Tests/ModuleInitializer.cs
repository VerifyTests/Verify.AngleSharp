public static class ModuleInitializer
{
    #region Initialize

    [ModuleInitializer]
    public static void Init() =>
        VerifyAngleSharpDiffing.Initialize();

    #endregion

    [ModuleInitializer]
    public static void InitOther()
    {
        VerifyDiffPlex.Initialize(OutputType.Compact);
        VerifierSettings.InitializePlugins();
        // Language resolves to the ambient UI culture, so it varies by machine
        VerifierSettings.IgnoreMembers("Length", "Index", "Language");
    }
}