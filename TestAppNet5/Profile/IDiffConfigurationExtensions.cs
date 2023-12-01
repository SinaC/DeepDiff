using DeepDiff.Configuration;
using TestAppNet5.Entities;

namespace TestAppNet5.Profile
{
    public static class IDiffConfigurationExtensions
    {
        public static IDiffEntityConfiguration<TEntity> PersistEntity<TEntity>(this IDiffConfiguration diffConfiguration)
            where TEntity : PersistEntity
        {
            return diffConfiguration.Entity<TEntity>()
                .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
        }

        public static IDiffEntityConfiguration<TEntity> AsPersistEntity<TEntity>(this IDiffEntityConfiguration<TEntity> diffEntityConfiguration)
            where TEntity : PersistEntity
        {
            return diffEntityConfiguration
                .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
        }
    }
}