using System;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Arc4u
{
    public abstract class PersistEntity : IPersistEntity
    {
        public virtual PersistChange PersistChange { get; set; }

        protected PersistEntity()
            : this(PersistChange.None)
        {
        }

        protected PersistEntity(PersistChange persistChange)
        {
            PersistChange = persistChange;
        }

        protected PersistEntity(PersistEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            PersistChange = entity.PersistChange;
        }
    }
}
