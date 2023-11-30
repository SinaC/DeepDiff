using System.Reflection;

namespace EntityComparer.Configuration
{
    public interface ICompareConfiguration
    {
        ICompareEntityConfiguration<TEntity> Entity<TEntity>()
            where TEntity : class;

        ICompareConfiguration AddProfile<TProfile>()
            where TProfile : CompareProfile;
        ICompareConfiguration AddProfiles(params Assembly[] assembliesToScan);

        ICompareConfiguration DisableHashtable();
        ICompareConfiguration SetHashtableThreshold(int threshold);

        IEntityComparer CreateComparer();
    }
}