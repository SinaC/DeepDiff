using System;
using System.Collections.Generic;

namespace EntityComparer.UnitTest.Entities.Simple;

internal class EntityLevel0 : PersistEntity
{
    public Guid Id { get; set; }

    public DateTime StartsOn { get; set; }
    public Direction Direction { get; set; }

    public decimal RequestedPower { get; set; }
    public decimal? Penalty { get; set; }

    public string AdditionalValueToCopy { get; set; } = null!;
    public string Comment { get; set; } = null!;

    // one-to-one
    public EntityLevel1 SubEntity { get; set; } = null!;

    // one-to-many
    public List<EntityLevel1> SubEntities { get; set; } = null!;

    // debug property, will never participate in compare neither as key nor value
    public int Index { get; set; }

}
