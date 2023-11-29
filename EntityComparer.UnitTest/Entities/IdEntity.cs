using System;

namespace EntityComparer.UnitTest.Entities
{
    public abstract class IdEntity : PersistEntity
    {
        public Guid Id { get; set; }

        protected IdEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}
