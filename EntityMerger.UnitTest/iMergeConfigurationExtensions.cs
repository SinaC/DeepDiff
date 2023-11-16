using EntityMerger.EntityMerger;
using EntityMerger.UnitTest.Entities;

namespace EntityMerger.UnitTest;

public static class IMergeConfigurationExtensions
{
    public static IMergeEntityConfiguration<TEntity> PersistEntity<TEntity>(this IMergeConfiguration mergeConfiguration)
        where TEntity : PersistEntity
    {
        return mergeConfiguration.Entity<TEntity>()
            .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
            .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
            .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
    }
}
