using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration;

internal sealed class MergeManyConfiguration : IMergeManyConfiguration
{
    public EngineConfiguration EngineConfiguration { get; }

    public MergeManyConfiguration()
    {
        EngineConfiguration = new EngineConfiguration();
    }

    public IMergeManyConfiguration HashtableThreshold(int threshold = 15)
    {
        EngineConfiguration.SetHashtableThreshold(threshold);
        return this;
    }

    public IMergeManyConfiguration ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(bool force = false)
    {
        EngineConfiguration.SetForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(force);
        return this;
    }

    public IMergeManyConfiguration SetCheckDuplicateKeys(bool checkDuplicateKeys = true)
    {
        EngineConfiguration.SetCheckDuplicateKeys(checkDuplicateKeys);
        return this;
    }
}
