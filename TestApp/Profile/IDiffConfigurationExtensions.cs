using DeepDiff.Configuration;
using TestApp.Entities;

namespace TestApp.Profile;

public static class IDiffConfigurationExtensions
{
    public static IEntityConfiguration<TEntity> PersistEntity<TEntity>(this IDeepDiffConfiguration diffConfiguration)
        where TEntity : PersistEntity
    {
        return diffConfiguration.Entity<TEntity>()
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
