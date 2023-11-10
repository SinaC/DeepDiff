using System;
using System.Collections.Generic;

namespace EntityMerger.UnitTest.Entities;

//
internal class Entity : PersistEntity
{
    public Guid Id { get; set; }
    public DateTime StartsOn { get; set; }
    public Direction Direction { get; set; }

    public decimal RequestedPower { get; set; }
    public decimal? Penalty { get; set; }

    public string Comment { get; set; } = null!;

    // one-to-one
    public SubEntity SubEntity { get; set; } = null!;

    // one-to-many
    public List<SubEntity> SubEntities { get; set; } = null!;

    // debug property, will never participate in merge neither as key nor value
    public int Index { get; set; }

}
