using System.Reflection;

namespace DeepDiff.Configuration
{
    public interface IDiffConfiguration
    {
        IDiffEntityConfiguration<TEntity> Entity<TEntity>()
            where TEntity : class;

        IDiffConfiguration AddProfile<TProfile>()
            where TProfile : DiffProfile;
        IDiffConfiguration AddProfiles(params Assembly[] assembliesToScan);

        IDiffConfiguration DisableHashtable();
        IDiffConfiguration SetHashtableThreshold(int threshold);

        IDeepDiff CreateDeepDiff();
    }
}