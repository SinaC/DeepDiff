using DeepDiff.Comparers;
using Xunit;

namespace DeepDiff.UnitTest.Comparer
{
    public class GenericToNonGenericEqualityComparerTests
    {
        [Fact]
        public void Decimal3_Same()
        {
            var comparer = GenericToNonGenericEqualityComparer.Create(new DecimalComparer(3));

            var equals = comparer.Equals(3.123456789m, 3.1234987654m);

            Assert.True(equals);
        }

        [Fact]
        public void Decimal_Different()
        {
            var comparer = GenericToNonGenericEqualityComparer.Create(new DecimalComparer(3));

            var equals = comparer.Equals(3.123456789m, 3.1934987654m);

            Assert.False(equals);
        }
    }
}
