using EntityMerger.EntityMerger;
using EntityMerger.UnitTest.Entities;

namespace EntityMerger.UnitTest;

public static class MergeConfigurationExtensions
{
    public static MergeEntityConfiguration<TEntityType> PersistEntity<TEntityType>(this MergeConfiguration mergeConfiguration)
        where TEntityType : PersistEntity
    {
        return mergeConfiguration.Entity<TEntityType>()
            .OnInsert(x => x.PersistChange, PersistChange.Insert)
            .OnUpdate(x => x.PersistChange, PersistChange.Update)
            .OnDelete(x => x.PersistChange, PersistChange.Delete);
    }
}
