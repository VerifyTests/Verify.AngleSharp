class CompareSettings(Action<IDiffingStrategyCollection> action)
{
    public Action<IDiffingStrategyCollection> Action { get; } = action;
}