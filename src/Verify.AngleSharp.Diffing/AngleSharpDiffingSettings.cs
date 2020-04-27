using System;
using System.Diagnostics.CodeAnalysis;
using AngleSharp.Diffing.Strategies;

namespace Verify
{
    public static class AngleSharpDiffingSettings
    {
        public static void PAngleSharpDiffingSettings(
            this VerifySettings settings,
            Action<IDiffingStrategyCollection> options)
        {
            Guard.AgainstNull(settings, nameof(settings));
            settings.Data["AngleSharpDiffing"] = new CompareSettings(options);
        }

        internal static bool GetCompareSettings(
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
    }
}