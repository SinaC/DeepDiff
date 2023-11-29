namespace TestApp.Entities;

public interface IAuditEntity<TAuditedBy, TAuditedOn>
{
    TAuditedOn AuditedOn { get; set; }
    TAuditedBy AuditedBy { get; set; }
}
