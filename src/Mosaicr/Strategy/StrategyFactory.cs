namespace Mosaicr.Strategy;

public static class StrategyFactory
{
    private static readonly Dictionary<string, Type> KnownStrategies = new(StringComparer.OrdinalIgnoreCase)
    {
        ["SimpleGridStrategy"] = typeof(SimpleGridStrategy),
        ["CircleFillingStrategy"] = typeof(CircleFillingStrategy)
    };

    public static IImageFillingStrategy Create(string strategyName, Dictionary<string, string> strategyMappings)
    {
        var classKey = $"{strategyName}_CLASS";

        if (!strategyMappings.TryGetValue(classKey, out var className))
            throw new ArgumentException(
                $"No strategy class mapping found for \"{strategyName}\". Expected key \"{classKey}\" in configuration.");

        if (!KnownStrategies.TryGetValue(className, out var strategyType))
            throw new ArgumentException(
                $"Unknown strategy implementation \"{className}\" for strategy \"{strategyName}\". " +
                $"Known implementations: {string.Join(", ", KnownStrategies.Keys)}");

        return (IImageFillingStrategy)Activator.CreateInstance(strategyType)!;
    }
}
