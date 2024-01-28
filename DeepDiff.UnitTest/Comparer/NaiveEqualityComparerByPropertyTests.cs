using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace DeepDiff.UnitTest.Comparer
{
    public class NaiveEqualityComparerByPropertyTests
    {
        [Fact]
        public void Decimal_Equal()
        {
            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => x.Power);

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
            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => x.Power);

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
            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => x.Price);

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
            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => x.Price);

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
            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => x.Price);

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
            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => x.Price);

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
            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => x.Price);

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
            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => x.Comment);

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
            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => x.Comment);

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
            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => x.Comment);

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
            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => x.Comment);

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
            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => x.Comment);

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

        [Fact]
        public void Decimal6_Different()
        {
            var typeSpecificComparers = new Dictionary<Type, IEqualityComparer>
            {
                { typeof(decimal), new DecimalComparer(6) }
            };

            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => x.Power, typeSpecificComparers);

            var existingEntity = new EntityLevel1
            {
                Power = 7.123456999m
            };
            var calculatedEntity = new EntityLevel1
            {
                Power = 7.123456789m
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.True(isEqual);
        }

        [Fact]
        public void NullableDecimal6_DifferentAfterSixthDecimal()
        {
            var typeSpecificComparers = new Dictionary<Type, IEqualityComparer>
            {
                { typeof(decimal?), new NullableDecimalComparer(6) }
            };

            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => x.Price, typeSpecificComparers);

            var existingEntity = new EntityLevel1
            {
                Price = 7.123456999m
            };
            var calculatedEntity = new EntityLevel1
            {
                Price = 7.123456789m
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.True(isEqual);
        }

        [Fact]
        public void NullableDecimal6_EqualNull()
        {
            var typeSpecificComparers = new Dictionary<Type, IEqualityComparer>
            {
                { typeof(decimal?), new NullableDecimalComparer(6) }
            };

            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => x.Price, typeSpecificComparers);

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
        public void NullableDecimal6_LeftNull()
        {
            var typeSpecificComparers = new Dictionary<Type, IEqualityComparer>
            {
                { typeof(decimal?), new NullableDecimalComparer(6) }
            };

            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => x.Price, typeSpecificComparers);

            var existingEntity = new EntityLevel1
            {
                Price = null
            };
            var calculatedEntity = new EntityLevel1
            {
                Price = 7.5m
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.False(isEqual);
        }

        [Fact]
        public void NullableDecimal6_RightNull()
        {
            var typeSpecificComparers = new Dictionary<Type, IEqualityComparer>
            {
                { typeof(decimal?), new NullableDecimalComparer(6) }
            };

            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => x.Price, typeSpecificComparers);

            var existingEntity = new EntityLevel1
            {
                Price = 7.5m
            };
            var calculatedEntity = new EntityLevel1
            {
                Price = null
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.False(isEqual);
        }

        [Fact]
        public void MultipleValues_Different()
        {
            var typeSpecificComparers = new Dictionary<Type, IEqualityComparer>
            {
                { typeof(decimal?), new NullableDecimalComparer(6) }
            };

            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => new { x.Power, x.Price }, typeSpecificComparers);

            var existingEntity = new EntityLevel1
            {
                Power = 9.123456789m,
                Price = 7.15m
            };
            var calculatedEntity = new EntityLevel1
            {
                Power = 9.1234567777m,
                Price = null
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.False(isEqual);
        }

        [Fact]
        public void MultipleValues_Equal()
        {
            var typeSpecificComparers = new Dictionary<Type, IEqualityComparer>
            {
                { typeof(decimal?), new NullableDecimalComparer(6) },
                { typeof(decimal), new DecimalComparer(6) }
            };

            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => new { x.Power, x.Price }, typeSpecificComparers);

            var existingEntity = new EntityLevel1
            {
                Power = 9.123456789m,
                Price = 7.15m
            };
            var calculatedEntity = new EntityLevel1
            {
                Power = 9.1234567777m,
                Price = 7.15m
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.True(isEqual);
        }

        [Fact]
        public void MultipleValues_DifferentAtSeventhDecimalsAndNoSpecificComparer()
        {
            var typeSpecificComparers = new Dictionary<Type, IEqualityComparer>
            {
                { typeof(decimal?), new NullableDecimalComparer(6) },
            };

            var comparer = new ComparerFactory<EntityLevel1>().CreateNaiveComparer(x => new { x.Power, x.Price }, typeSpecificComparers);

            var existingEntity = new EntityLevel1
            {
                Power = 9.123456789m, // will not used NullableDecimalComparer (should have added DecimalComparer at init)
                Price = 7.123456789m, // will use NullableDecimalComparer
            };
            var calculatedEntity = new EntityLevel1
            {
                Power = 9.1234567777m, // will be considered as different
                Price = 7.1234567777m // will be considered as same
            };

            var isEqual = comparer.Equals(existingEntity, calculatedEntity);

            Assert.False(isEqual);
        }
    }
}
