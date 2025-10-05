using DeepDiff.UnitTest.Entities;

namespace DeepDiff.UnitTest.Inheritance.Entities.NonAbstract
{
    public class Entity : PersistEntity
    {
        public int Key { get; set; }
        public string Name { get; set; } = null!;

        public List<SubEntity> SubEntities { get; set; } = null!;
    }
}
