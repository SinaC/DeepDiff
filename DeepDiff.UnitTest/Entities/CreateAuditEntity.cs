using System;

namespace DeepDiff.UnitTest.Entities;

public abstract class CreateAuditEntity<TId> : IdEntity<TId>
    where TId : struct
{
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
}
