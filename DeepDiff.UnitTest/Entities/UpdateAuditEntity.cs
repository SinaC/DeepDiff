using System;

namespace DeepDiff.UnitTest.Entities;

public abstract class UpdateAuditEntity<TId> : CreateAuditEntity<TId>
    where TId: struct
{
    public string UpdatedBy { get; set; } = null!;
    public DateTime? UpdatedOn { get; set; }
}
