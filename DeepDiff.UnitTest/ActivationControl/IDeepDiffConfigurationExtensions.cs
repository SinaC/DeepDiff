using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using System;

namespace DeepDiff.UnitTest.ActivationControl
{
    internal static class IDeepDiffConfigurationExtensions
    {
        public static IEntityConfiguration<TEntity> PersistEntity<TEntity>(this IDeepDiffConfiguration deepDiffConfiguration)
            where TEntity : PersistEntity
        {
            return deepDiffConfiguration.Entity<TEntity>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .WithComparer(new DecimalComparer(6))
                .WithComparer(new NullableDecimalComparer(6))
                .Ignore(x => x.PersistChange);
        }

        public static IEntityConfiguration<TEntity> IgnoreId<TEntity>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : IdEntity<int>
        {
            return entityConfiguration
                .Ignore(x => new { x.PersistChange, x.Id });
        }

        public static IEntityConfiguration<TEntity> IgnoreCreateAudit<TEntity>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : CreateAuditEntity<int>
        {
            return entityConfiguration
                .Ignore(x => new { x.PersistChange, x.Id, x.CreatedBy, x.CreatedOn });
        }

        public static IEntityConfiguration<TEntity> IgnoreUpdateAudit<TEntity>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : UpdateAuditEntity<int>
        {
            return entityConfiguration
                .Ignore(x => new { x.PersistChange, x.Id, x.CreatedBy, x.CreatedOn, x.UpdatedBy, x.UpdatedOn });
        }

        public static IEntityConfiguration<TEntity> IgnoreAudit<TEntity>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : class, IAuditEntity<string, DateTime>
        {
            return entityConfiguration
                .Ignore(x => new { x.AuditedBy, x.AuditedOn });
        }
    }
}
