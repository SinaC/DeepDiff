namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Arc4u
{
    public interface IUpdateAuditEntity<TUpdatedBy, TUpdatedOn>
    {
        TUpdatedBy UpdatedBy { get; set; }

        TUpdatedOn UpdatedOn { get; set; }
    }

    public interface IUpdateAuditEntity<TCreatedBy, TCreatedOn, TUpdatedBy, TUpdatedOn> : ICreateAuditEntity<TCreatedBy, TCreatedOn>, IUpdateAuditEntity<TUpdatedBy, TUpdatedOn>
    {
    }
}
