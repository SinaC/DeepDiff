using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration;

internal sealed class MergeSingleConfiguration : IMergeSingleConfiguration
{
    public EngineConfiguration EngineConfiguration { get; }

    public MergeSingleConfiguration()
    {
        EngineConfiguration = new EngineConfiguration();
    }

    public IMergeSingleConfiguration HashtableThreshold(int threshold = 15)
    {
        EngineConfiguration.SetHashtableThreshold(threshold);
        return this;
    }

    public IMergeSingleConfiguration ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(bool force = false)
    {
        EngineConfiguration.SetForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(force);
        return this;
    }

    public IMergeSingleConfiguration SetCheckDuplicateKeys(bool checkDuplicateKeys = true)
    {
        EngineConfiguration.SetCheckDuplicateKeys(checkDuplicateKeys);
        return this;
    }
}
