public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        #region Initialize

        VerifyAngleSharpDiffing.Initialize();

        #endregion
    }
}