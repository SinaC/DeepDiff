using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace DeepDiff.UnitTest.Comparer
{
    public class NaiveEqualityComparerByPropertyCompareTests
    {
        [Fact]
        public void TypeAndPropertyInfoSpecificComparer_3Decimals()
        {
            var comparerFactory = new ComparerFactory<EntityLevel1>();

            var typeSpecificComparers = new Dictionary<Type, object>
            {
                { typeof(decimal?), new NullableDecimalComparer(3) },
            };

            var propertyInfoSpecificComparers = new Dictionary<PropertyInfo, object>
            {
                { comparerFactory.GetPropertyInfo(x => x.Price), new NullableDecimalComparer(6) }
            };

            var comparer = comparerFactory.CreateNaiveComparer(x => x.Price, typeSpecificComparers, propertyInfoSpecificComparers);

            var existingEntity = new EntityLevel1
            {
                Price = 7.1234500000m,
            };
            var calculatedEntity = new EntityLevel1
            {
                Price = 7.1234599999m
            };

            var result = comparer.Compare(existingEntity, calculatedEntity);

            Assert.NotNull(result);
            Assert.False(result.IsEqual);
            Assert.Single(result.Details!);
            Assert.Equal(nameof(EntityLevel1.Price), result.Details!.Single().PropertyInfo.Name);
            Assert.Equal((object)7.1234500000m, result.Details!.Single().OldValue);
            Assert.Equal((object)7.1234599999m, result.Details!.Single().NewValue);
        }
    }
}
