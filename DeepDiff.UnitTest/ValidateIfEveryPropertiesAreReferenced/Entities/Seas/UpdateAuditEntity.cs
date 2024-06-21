using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Arc4u;
using System;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Seas
{
    public class UpdateAuditEntity : UpdateAuditEntity<Guid, string, DateTime, string, DateTime?>
    {
        public UpdateAuditEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}
