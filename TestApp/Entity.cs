using EntityMerger;

namespace TestApp
{
    public class Entity : PersistEntity
    {
        public Guid Id { get; set; }
        public DateTime StartsOn { get; set; }
        public Direction Direction { get; set; }

        public decimal RequestedPower { get; set; }
        public decimal? Penalty { get; set; }

        public string Comment { get; set; }

        public List<SubEntity> SubEntities { get; set; }

    }
    


}
