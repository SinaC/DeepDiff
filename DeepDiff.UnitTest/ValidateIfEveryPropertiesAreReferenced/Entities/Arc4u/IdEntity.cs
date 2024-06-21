using System;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Arc4u
{
    public class IdEntity : IdEntity<Guid>
    {
        public IdEntity()
            : this(PersistChange.None)
        {
        }

        protected IdEntity(PersistChange persistChange)
            : base(persistChange)
        {
            Id = Guid.NewGuid();
        }
    }
}
