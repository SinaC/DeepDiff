namespace DeepDiff.UnitTest.Entities;

public abstract class IdEntity<TId> : PersistEntity
    where TId : struct
{
    public TId Id { get; set; }
}
