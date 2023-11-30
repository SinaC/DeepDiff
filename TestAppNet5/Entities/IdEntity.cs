using System;

namespace TestAppNet5.Entities
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