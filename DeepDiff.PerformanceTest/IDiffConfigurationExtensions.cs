using DeepDiff.Configuration;
using DeepDiff.PerformanceTest.Entities;

namespace DeepDiff.PerformanceTest;

public static class IDiffConfigurationExtensions
{
    public static IEntityConfiguration<TEntity> PersistEntity<TEntity>(this IDeepDiffConfiguration diffConfiguration)
        where TEntity : PersistEntity
    {
        return diffConfiguration.ConfigureEntity<TEntity>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));
    }

    public static IEntityConfiguration<TEntity> PersistEntity<TEntity>(this IEntityConfiguration<TEntity> entityConfiguration)
        where TEntity : PersistEntity
    {
        return entityConfiguration
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));
    }
}
