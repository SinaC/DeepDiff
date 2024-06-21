namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Arc4u
{
    public interface ICreateAuditEntity<TCreatedBy, TCreatedOn>
    {
        TCreatedBy CreatedBy { get; set; }

        TCreatedOn CreatedOn { get; set; }
    }
}
