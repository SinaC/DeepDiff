using System.Reflection;

namespace EntityMerger.Configuration;

public interface IMergeConfiguration
{
    IMergeEntityConfiguration<TEntity> Entity<TEntity>()
        where TEntity : class;

    IMergeConfiguration AddProfile<TProfile>()
        where TProfile : MergeProfile;
    IMergeConfiguration AddProfiles(params Assembly[] assembliesToScan);

    IMergeConfiguration DisableHashtable();
    IMergeConfiguration SetHashtableThreshold(int threshold);

    IMerger CreateMerger();
}
