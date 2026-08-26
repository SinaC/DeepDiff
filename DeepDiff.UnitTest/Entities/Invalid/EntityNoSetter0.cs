namespace DeepDiff.UnitTest.Entities.Invalid;

internal class EntityNoSetter0 : PersistEntity
{
    public Guid Id { get; set; }

    public DateTime StartsOn { get; set; }

    public decimal RequestedPower { get; set; }
    public decimal? Penalty { get; set; }

    public string AdditionalValueToCopy { get; set; } = null!;
    public string Comment { get; set; } = null!;

    // one-to-one
    public EntityNoSetter1 SubEntity { get; } = null!; // read-only

    // one-to-many
    public List<EntityNoSetter1> SubEntities { get; } = []; // read-only

    // debug property, will never participate in compare neither as key nor value
    public int Index { get; set; }

}
