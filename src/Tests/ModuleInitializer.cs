public static class ModuleInitializer
{
    #region Initialize

    [ModuleInitializer]
    public static void Init()
    {
        VerifyAngleSharpDiffing.Initialize();

        #endregion

        VerifyDiffPlex.Initialize();
    }
}