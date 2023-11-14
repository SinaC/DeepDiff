using EntityMerger.EntityMerger;
using EntityMerger.UnitTest.Entities;

namespace EntityMerger.UnitTest;

public static class IMergeConfigurationExtensions
{
    public static IMergeEntityConfiguration<TEntityType> PersistEntity<TEntityType>(this IMergeConfiguration mergeConfiguration)
        where TEntityType : PersistEntity
    {
        return mergeConfiguration.Entity<TEntityType>()
            .OnInsert(x => x.PersistChange, PersistChange.Insert)
            .OnUpdate(x => x.PersistChange, PersistChange.Update)
            .OnDelete(x => x.PersistChange, PersistChange.Delete);
    }
}
