using System;
using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Arc4u;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Seas
{
    public abstract class AuditEntity : AuditEntity<Guid, string, DateTime>, IAuditEntity<string, DateTime>
    {
        public AuditEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}
