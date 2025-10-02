using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Arc4u;
using System;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Seas
{
    public class CreateAuditEntity : CreateAuditEntity<Guid, string, DateTime>
    {
        public CreateAuditEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}
