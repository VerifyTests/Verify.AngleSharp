﻿public static class ModuleInitializer
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
        VerifierSettings.IgnoreMembers("Length", "Index");
    }
}