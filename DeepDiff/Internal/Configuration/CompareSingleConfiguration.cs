using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration;

internal sealed class CompareSingleConfiguration : ICompareSingleConfiguration
{
    public EngineConfiguration EngineConfiguration { get; }

    public CompareSingleConfiguration()
    {
        EngineConfiguration = new EngineConfiguration();
        EngineConfiguration.SetForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(false);
        EngineConfiguration.SetCompareOnly(true);
    }

    public ICompareSingleConfiguration HashtableThreshold(int threshold = 15)
    {
        EngineConfiguration.SetHashtableThreshold(threshold);
        return this;
    }

    public ICompareSingleConfiguration SetCheckDuplicateKeys(bool checkDuplicateKeys = true)
    {
        EngineConfiguration.SetCheckDuplicateKeys(checkDuplicateKeys);
        return this;
    }
}
