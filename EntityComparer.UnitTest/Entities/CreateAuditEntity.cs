using System;

namespace EntityComparer.UnitTest.Entities
{
    public abstract class CreateAuditEntity : IdEntity
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
