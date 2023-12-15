using System.Reflection;

namespace DeepDiff.Configuration
{
    public interface IDiffConfiguration
    {
        IDiffEntityConfiguration<TEntity> Entity<TEntity>()
            where TEntity : class;

        IDiffConfiguration AddProfile<TProfile>()
            where TProfile : DiffProfile;
        IDiffConfiguration AddProfile(DiffProfile diffProfile);
        IDiffConfiguration AddProfiles(params Assembly[] assembliesToScan);

        IDeepDiff CreateDeepDiff();

        void ValidateConfiguration();
    }
}