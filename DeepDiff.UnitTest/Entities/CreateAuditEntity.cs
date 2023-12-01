using System;

namespace DeepDiff.UnitTest.Entities;

public abstract class CreateAuditEntity : IdEntity
{
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
}
