using System.Reflection;

namespace DeepDiff.Configuration
{
    public interface IDeepDiffConfiguration
    {
        IEntityConfiguration<TEntity> Entity<TEntity>()
            where TEntity : class;

        IDeepDiffConfiguration AddProfile<TProfile>()
            where TProfile : DiffProfile;
        IDeepDiffConfiguration AddProfile(DiffProfile diffProfile);
        IDeepDiffConfiguration AddProfiles(params Assembly[] assembliesToScan);

        IDeepDiff CreateDeepDiff();

        void ValidateConfiguration();
    }
}