namespace TestAppNet6.Entities;

public abstract class CreateAuditEntity : IdEntity
{
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
}
