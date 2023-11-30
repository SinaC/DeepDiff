using System;

namespace TestAppNet5.Entities
{
    public abstract class CreateAuditEntity : IdEntity
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}