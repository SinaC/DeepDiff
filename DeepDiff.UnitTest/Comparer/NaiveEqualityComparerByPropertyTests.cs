using DeepDiff.Comparers;
using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities.Simple;
using Xunit;

namespace DeepDiff.UnitTest.Comparer
{
    // TODO: perform exactly same test with PrecompileEqualityComparer
    public class NaiveEqualityComparerByPropertyTests
    {
        [Fact]
        public void Decimal_Equal()
        {
            var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
            diffEntityConfiguration.HasValues(x => x.Power);
            var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.ValuesConfiguration.ValuesProperties);

            var existingEntity = new EntityLevel1
            {
                Power = 7
            };
            var calculatedEntity = new EntityLevel1
            {
                Power = 7
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.True(isEqual);
        }

        [Fact]
        public void Decimal_Different()
        {
            var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
            diffEntityConfiguration.HasValues(x => x.Power);
            var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.ValuesConfiguration.ValuesProperties);

            var existingEntity = new EntityLevel1
            {
                Power = 7
            };
            var calculatedEntity = new EntityLevel1
            {
                Power = 10
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.False(isEqual);
        }

        [Fact]
        public void NullableDecimal_Equal_NonNull()
        {
            var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
            diffEntityConfiguration.HasValues(x => x.Price);
            var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.ValuesConfiguration.ValuesProperties);

            var existingEntity = new EntityLevel1
            {
                Price = 7
            };
            var calculatedEntity = new EntityLevel1
            {
                Price = 7
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.True(isEqual);
        }

        [Fact]
        public void NullableDecimal_Equal_Null()
        {
            var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
            diffEntityConfiguration.HasValues(x => x.Price);
            var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.ValuesConfiguration.ValuesProperties);

            var existingEntity = new EntityLevel1
            {
                Price = null
            };
            var calculatedEntity = new EntityLevel1
            {
                Price = null
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.True(isEqual);
        }

        [Fact]
        public void NullableDecimal_Different_LeftNull()
        {
            var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
            diffEntityConfiguration.HasValues(x => x.Price);
            var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.ValuesConfiguration.ValuesProperties);

            var existingEntity = new EntityLevel1
            {
                Price = null
            };
            var calculatedEntity = new EntityLevel1
            {
                Price = 7
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.False(isEqual);
        }

        [Fact]
        public void NullableDecimal_Different_RightNull()
        {
            var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
            diffEntityConfiguration.HasValues(x => x.Price);
            var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.ValuesConfiguration.ValuesProperties);

            var existingEntity = new EntityLevel1
            {
                Price = 7
            };
            var calculatedEntity = new EntityLevel1
            {
                Price = null
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.False(isEqual);

        }

        [Fact]
        public void NullableDecimal_Different_NonNull()
        {
            var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
            diffEntityConfiguration.HasValues(x => x.Price);
            var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.ValuesConfiguration.ValuesProperties);

            var existingEntity = new EntityLevel1
            {
                Price = 7
            };
            var calculatedEntity = new EntityLevel1
            {
                Price = 9
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.False(isEqual);
        }

        //
        [Fact]
        public void String_Equal_NonNull()
        {
            var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
            diffEntityConfiguration.HasValues(x => x.Comment);
            var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.ValuesConfiguration.ValuesProperties);

            var existingEntity = new EntityLevel1
            {
                Comment = "7"
            };
            var calculatedEntity = new EntityLevel1
            {
                Comment = "7"
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.True(isEqual);
        }

        [Fact]
        public void String_Equal_Null()
        {
            var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
            diffEntityConfiguration.HasValues(x => x.Comment);
            var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.ValuesConfiguration.ValuesProperties);

            var existingEntity = new EntityLevel1
            {
                Comment = null
            };
            var calculatedEntity = new EntityLevel1
            {
                Comment = null
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.True(isEqual);
        }

        [Fact]
        public void String_Different_LeftNull()
        {
            var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
            diffEntityConfiguration.HasValues(x => x.Comment);
            var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.ValuesConfiguration.ValuesProperties);

            var existingEntity = new EntityLevel1
            {
                Comment = null
            };
            var calculatedEntity = new EntityLevel1
            {
                Comment = "7"
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.False(isEqual);
        }

        [Fact]
        public void String_Different_RightNull()
        {
            var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
            diffEntityConfiguration.HasValues(x => x.Comment);
            var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.ValuesConfiguration.ValuesProperties);

            var existingEntity = new EntityLevel1
            {
                Comment = "7"
            };
            var calculatedEntity = new EntityLevel1
            {
                Comment = null
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.False(isEqual);

        }

        [Fact]
        public void String_Different_NonNull()
        {
            var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
            diffEntityConfiguration.HasValues(x => x.Comment);
            var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.ValuesConfiguration.ValuesProperties);

            var existingEntity = new EntityLevel1
            {
                Comment = "7"
            };
            var calculatedEntity = new EntityLevel1
            {
                Comment = "9"
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.False(isEqual);
        }
    }
}
