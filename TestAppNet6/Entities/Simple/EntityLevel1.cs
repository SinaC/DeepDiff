namespace TestAppNet6.Entities.Simple
{
    internal class EntityLevel1 : PersistEntity
    {
        public Guid Id { get; set; }

        public DateTime Timestamp { get; set; }

        public decimal Power { get; set; }
        public decimal? Price { get; set; }

        public string Comment { get; set; } = null!;

        // one-to-one
        public EntityLevel2 SubEntity { get; set; } = null!;

        // one-to-many
        public List<EntityLevel2> SubEntities { get; set; } = null!;

        // debug property, will never participate in compare neither as key nor value
        public int Index { get; set; }
    }
}