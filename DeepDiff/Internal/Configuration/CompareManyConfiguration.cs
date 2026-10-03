using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration;

internal sealed class CompareManyConfiguration : ICompareManyConfiguration
{
    public EngineConfiguration EngineConfiguration { get; }

    public CompareManyConfiguration()
    {
        EngineConfiguration = new EngineConfiguration();
        EngineConfiguration.SetForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(false);
        EngineConfiguration.SetCompareOnly(true);
    }

    public ICompareManyConfiguration HashtableThreshold(int threshold = 15)
    {
        EngineConfiguration.SetHashtableThreshold(threshold);
        return this;
    }

    public ICompareManyConfiguration SetCheckDuplicateKeys(bool checkDuplicateKeys = true)
    {
        EngineConfiguration.SetCheckDuplicateKeys(checkDuplicateKeys);
        return this;
    }
}
