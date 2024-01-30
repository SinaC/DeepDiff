using System;
using System.Collections.Generic;
using Xunit;

namespace DeepDiff.UnitTest.Comparer
{
    public class CastingTests
    {
        [Fact]
        public void CastFromObjectToIEqualityComparerOfT()
        {
            var d1 = 7.1234500000m;
            var d2 = 7.1234599999m;

            var comparer = new DecimalComparer(6);

            var equalityComparer = (object)comparer;

            Type equalityComparerType = typeof(IEqualityComparer<>).MakeGenericType(typeof(decimal));
            if (equalityComparerType.IsAssignableFrom(equalityComparer.GetType()))
            {
                var equalMethod = equalityComparerType.GetMethod(nameof(Equals), new[] { typeof(decimal), typeof(decimal) });
                var result = (bool)equalMethod.Invoke(equalityComparer, new object[] { d1, d2 });
            }
        }
    }
}
