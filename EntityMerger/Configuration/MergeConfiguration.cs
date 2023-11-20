using System.Reflection;

namespace EntityMerger.Configuration;

public sealed class MergeConfiguration : IMergeConfiguration
{
    internal Dictionary<Type, MergeEntityConfiguration> MergeEntityConfigurations { get; private set; } = new Dictionary<Type, MergeEntityConfiguration>();
    internal bool UseHashtable { get; private set; } = true;
    internal int HashtableThreshold { get; private set; } = 15;

    public IMergeEntityConfiguration<TEntity> Entity<TEntity>()
        where TEntity : class
    {
        var mergeEntityConfiguration = new MergeEntityConfiguration(typeof(TEntity));
        MergeEntityConfigurations.Add(typeof(TEntity), mergeEntityConfiguration);

        return new MergeEntityConfiguration<TEntity>(mergeEntityConfiguration);
    }

    public IMergeConfiguration AddProfile<TProfile>()
        where TProfile : MergeProfile
    {
        var mergeProfileInstance = (MergeProfile)Activator.CreateInstance(typeof(TProfile))!;
        foreach (var typeAndMergeEntityConfiguration in mergeProfileInstance.MergeEntityConfigurations)
            MergeEntityConfigurations.Add(typeAndMergeEntityConfiguration.Key, typeAndMergeEntityConfiguration.Value);
        return this;
    }

    public IMergeConfiguration AddProfiles(params Assembly[] assembliesToScan)
    {
        foreach (var assembly in assembliesToScan)
        {
            var mergeProfileType = typeof(MergeProfile);
            foreach (var derivedMergeProfileType in assembly.GetTypes().Where(x => x != mergeProfileType && mergeProfileType.IsAssignableFrom(x)))
            {
                var mergeProfileInstance = (MergeProfile)Activator.CreateInstance(derivedMergeProfileType)!;
                foreach (var typeAndMergeEntityConfiguration in mergeProfileInstance.MergeEntityConfigurations)
                    MergeEntityConfigurations.Add(typeAndMergeEntityConfiguration.Key, typeAndMergeEntityConfiguration.Value);
            }
        }
        return this;
    }

    public IMergeConfiguration DisableHashtable()
    {
        UseHashtable = false;
        return this;
    }

    public IMergeConfiguration SetHashtableThreshold(int threshold)
    {
        HashtableThreshold = threshold;
        return this;
    }

    public IMerger CreateMerger()
    {
        // TODO: validate configuration (Keys cannot be empty + InsertAssignValue/UpdateAssignValue/DeleteAssignValue cannot be null, ...)

        return new Merger(this);
    }
}
