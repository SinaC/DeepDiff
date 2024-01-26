using System;

namespace DeepDiff.UnitTest.Entities.Simple;

internal class EntityLevel2 : PersistEntity
{
    public Guid Id { get; set; }

    public string DeliveryPointEan { get; set; } = null!;

    public decimal Value1 { get; set; }
    public decimal? Value2 { get; set; }

    // FK to EntityLevel1
    public Guid EntityLevel1Id { get; set; }
    public EntityLevel1 EntityLevel1 { get; set; } = null!;


    // debug property, will never participate in compare neither as key nor value
    public int Index { get; set; }
}
