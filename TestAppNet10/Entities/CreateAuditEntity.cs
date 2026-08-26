namespace TestAppNet8.Entities;

public abstract class CreateAuditEntity : IdEntity
{
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
}
