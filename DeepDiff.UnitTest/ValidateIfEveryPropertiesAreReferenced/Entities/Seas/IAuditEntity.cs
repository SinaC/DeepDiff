namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Seas
{
    public interface IAuditEntity<TAuditedBy, TAuditedOn>
    {
        TAuditedBy AuditedBy { get; set; }
        TAuditedOn AuditedOn { get; set; }
    }
}
