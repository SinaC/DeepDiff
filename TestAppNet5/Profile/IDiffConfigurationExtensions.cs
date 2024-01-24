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
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));
        }

        public static IDiffEntityConfiguration<TEntity> PersistEntity<TEntity>(this IDiffEntityConfiguration<TEntity> diffEntityConfiguration)
            where TEntity : PersistEntity
        {
            return diffEntityConfiguration
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));
        }
    }
}