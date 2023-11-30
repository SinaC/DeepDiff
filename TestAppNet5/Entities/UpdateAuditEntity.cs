using System;

namespace TestAppNet5.Entities
{
    public abstract class UpdateAuditEntity : CreateAuditEntity
    {
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}