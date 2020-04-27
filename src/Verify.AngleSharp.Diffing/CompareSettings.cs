using System;
using AngleSharp.Diffing.Strategies;

class CompareSettings
{
    public Action<IDiffingStrategyCollection> Action { get; }

    public CompareSettings(Action<IDiffingStrategyCollection> action)
    {
        Action = action;
    }
}