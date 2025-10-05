using DeepDiff.Configuration;
using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Arc4u;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Extensions
{
    public static class IDiffConfigurationExtensions
    {
        public static IEntityConfiguration<TEntity> PersistEntity<TEntity>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : PersistEntity
        {
            return entityConfiguration
                .WithComparer(new DecimalComparer(6))
                .WithComparer(new NullableDecimalComparer(6))
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));
        }

        // domain entities (TId, string, datetime)
        public static IEntityConfiguration<TEntity> CreateAuditEntity<TEntity, TId>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : CreateAuditEntity<TId, string, DateTime>
        {
            return entityConfiguration
                .PersistEntity()
                .IgnoreId<TEntity, TId>()
                .IgnoreCreateAudit<TEntity, string, DateTime>();
        }

        public static IEntityConfiguration<TEntity> UpdateAuditEntity<TEntity, TId>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : UpdateAuditEntity<TId, string, DateTime, string, DateTime?>
        {
            return entityConfiguration
                .PersistEntity()
                .IgnoreId<TEntity, TId>()
                .IgnoreCreateAudit()
                .IgnoreUpdateAudit();
        }

        public static IEntityConfiguration<TEntity> AuditEntity<TEntity, TId>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : AuditEntity<TId, string, DateTime>, Entities.Seas.IAuditEntity<string, DateTime>
        {
            return entityConfiguration
                .PersistEntity()
                .IgnoreId<TEntity, TId>()
                .IgnoreAudit();
        }

        // domain entities (guid, string, datetime)
        public static IEntityConfiguration<TEntity> CreateAuditEntity<TEntity>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : Entities.Seas.CreateAuditEntity
        {
            return entityConfiguration
                .CreateAuditEntity<TEntity, Guid>();
        }

        public static IEntityConfiguration<TEntity> UpdateAuditEntity<TEntity>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : Entities.Seas.UpdateAuditEntity
        {
            return entityConfiguration
                .UpdateAuditEntity<TEntity, Guid>();
        }

        public static IEntityConfiguration<TEntity> AuditEntity<TEntity>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : Entities.Seas.AuditEntity
        {
            return entityConfiguration
                //.AuditEntity<TEntity, Guid, string, DateTime>() cannot be used because IAuditEntity is not declared in arc4u and arc4u.AuditedEntity is not inheriting from seas.IAuditEntity
                .PersistEntity()
                .IgnoreId<TEntity, Guid>()
                .Ignore(x => new { x.AuditedBy, x.AuditedOn });
        }

        // arc4u entities
        public static IEntityConfiguration<TEntity> IdEntity<TEntity>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : IdEntity
        {
            return entityConfiguration
                .PersistEntity()
                .IgnoreId<TEntity, Guid>();
        }

        public static IEntityConfiguration<TEntity> CreateAuditEntity<TEntity, TId, TCreatedBy, TCreatedOn>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : CreateAuditEntity<TId, TCreatedBy, TCreatedOn>
        {
            return entityConfiguration
                .PersistEntity()
                .IgnoreId<TEntity, TId>()
                .IgnoreCreateAudit<TEntity, TCreatedBy, TCreatedOn>();
        }

        public static IEntityConfiguration<TEntity> UpdateAuditEntity<TEntity, TId, TCreatedBy, TCreatedOn, TUpdatedBy, TUpdatedOn>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : UpdateAuditEntity<TId, TCreatedBy, TCreatedOn, TUpdatedBy, TUpdatedOn>
        {
            return entityConfiguration
                .PersistEntity()
                .IgnoreId<TEntity, TId>()
                .IgnoreCreateAudit<TEntity, TCreatedBy, TCreatedOn>()
                .IgnoreUpdateAudit<TEntity, TUpdatedBy, TUpdatedOn>();
        }

        public static IEntityConfiguration<TEntity> AuditEntity<TEntity, TId, TAuditedBy, TAuditedOn>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : AuditEntity<TId, TAuditedBy, TAuditedOn>, Entities.Seas.IAuditEntity<TAuditedBy, TAuditedOn>
        {
            return entityConfiguration
                .PersistEntity()
                .IgnoreId<TEntity, TId>()
                .IgnoreAudit<TEntity, TAuditedBy, TAuditedOn>();
        }

        // arc4u interfaces (string, datetime)
        public static IEntityConfiguration<TEntity> IgnoreCreateAudit<TEntity>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : class, ICreateAuditEntity<string, DateTime>
        {
            return entityConfiguration
                .Ignore(x => new { x.CreatedBy, x.CreatedOn });
        }

        public static IEntityConfiguration<TEntity> IgnoreUpdateAudit<TEntity>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : class, IUpdateAuditEntity<string, DateTime?>
        {
            return entityConfiguration
                .Ignore(x => new { x.UpdatedBy, x.UpdatedOn });
        }

        public static IEntityConfiguration<TEntity> IgnoreAudit<TEntity>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : class, Entities.Seas.IAuditEntity<string, DateTime>
        {
            return entityConfiguration
                .Ignore(x => new { x.AuditedBy, x.AuditedOn });
        }

        // arc4u interfaces
        public static IEntityConfiguration<TEntity> IgnoreId<TEntity, TId>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : class, IIdEntity<TId>
        {
            return entityConfiguration
                .Ignore(x => x.Id);
        }

        public static IEntityConfiguration<TEntity> IgnoreCreateAudit<TEntity, TCreatedBy, TCreatedOn>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : class, ICreateAuditEntity<TCreatedBy, TCreatedOn>
        {
            return entityConfiguration
                .Ignore(x => new { x.CreatedBy, x.CreatedOn });
        }

        public static IEntityConfiguration<TEntity> IgnoreUpdateAudit<TEntity, TUpdatedBy, TUpdatedOn>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : class, IUpdateAuditEntity<TUpdatedBy, TUpdatedOn>
        {
            return entityConfiguration
                .Ignore(x => new { x.UpdatedBy, x.UpdatedOn });
        }

        public static IEntityConfiguration<TEntity> IgnoreAudit<TEntity, TAuditedBy, TAuditedOn>(this IEntityConfiguration<TEntity> entityConfiguration)
            where TEntity : class, Entities.Seas.IAuditEntity<TAuditedBy, TAuditedOn>
        {
            return entityConfiguration
                .Ignore(x => new { x.AuditedBy, x.AuditedOn });
        }
    }
}
