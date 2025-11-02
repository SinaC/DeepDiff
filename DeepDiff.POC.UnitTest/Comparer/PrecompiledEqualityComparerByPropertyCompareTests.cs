using DeepDiff.POC.UnitTest.Entities.Simple;
using System.Reflection;
using Xunit;

namespace DeepDiff.POC.UnitTest.Comparer
{
    public class PrecompiledEqualityComparerByPropertyCompareTests
    {
        [Fact]
        public void TypeAndPropertyInfoSpecificComparer_Compare_3Decimals()
        {
            var comparerFactory = new ComparerFactory<EntityLevel1>();

            var typeSpecificComparers = new Dictionary<Type, object>
            {
                { typeof(decimal?), new NullableDecimalComparer(6) },
            };

            var propertyInfoSpecificComparers = new Dictionary<PropertyInfo, object>
            {
                { comparerFactory.GetPropertyInfo(x => x.Price), new NullableDecimalComparer(3) }
            };

            var comparer = comparerFactory.CreatePrecompiledComparer(x => x.Price, typeSpecificComparers, propertyInfoSpecificComparers);

            var existingEntity = new EntityLevel1
            {
                Price = 7.1234500000m,
            };
            var calculatedEntity = new EntityLevel1
            {
                Price = 7.1234599999m
            };

            var result = comparer.Compare(existingEntity, calculatedEntity); // decimal(3) will have higher priority than decimal(6) because defined at property level

            Assert.NotNull(result);
            Assert.True(result.IsEqual);
            Assert.Empty(result.Details!);
        }

        [Fact]
        public void TypeAndPropertyInfoSpecificComparer_Compare_6Decimals()
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

            var comparer = comparerFactory.CreatePrecompiledComparer(x => x.Price, typeSpecificComparers, propertyInfoSpecificComparers);

            var existingEntity = new EntityLevel1
            {
                Price = 7.1234500000m,
            };
            var calculatedEntity = new EntityLevel1
            {
                Price = 7.1234599999m
            };

            var result = comparer.Compare(existingEntity, calculatedEntity); // decimal(6) will have higher priority than decimal(3) because defined at property level

            Assert.NotNull(result);
            Assert.False(result.IsEqual);
            Assert.Single(result.Details!);
            Assert.Equal(nameof(EntityLevel1.Price), result.Details!.Single().PropertyInfo.Name);
            Assert.Equal((object)7.1234500000m, result.Details!.Single().OldValue);
            Assert.Equal((object)7.1234599999m, result.Details!.Single().NewValue);
        }
    }
}
