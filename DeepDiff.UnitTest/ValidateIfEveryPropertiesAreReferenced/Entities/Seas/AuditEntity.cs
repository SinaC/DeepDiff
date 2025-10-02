using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Arc4u;
using System;

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
