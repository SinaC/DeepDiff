using EntityComparer.Configuration;
using TestApp.Entities;

namespace TestApp.Profile;

public static class ICompareConfigurationExtensions
{
    public static ICompareEntityConfiguration<TEntity> PersistEntity<TEntity>(this ICompareConfiguration compareConfiguration)
        where TEntity : PersistEntity
    {
        return compareConfiguration.Entity<TEntity>()
            .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
            .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
            .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
    }

    public static ICompareEntityConfiguration<TEntity> AsPersistEntity<TEntity>(this ICompareEntityConfiguration<TEntity> compareEntityConfiguration)
        where TEntity : PersistEntity
    {
        return compareEntityConfiguration
            .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
            .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
            .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
    }
}
