using System.Collections;

namespace DeepDiff.UnitTest
{
    public class OrdinalIgnoreCaseStringComparer : IEqualityComparer
    {
        public new bool Equals(object? left, object? right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (left is not string)
                return false;
            if (right is not string)
                return false;
            var leftAsString = (string)left;
            var rightAsString = (string)right;
            return string.Compare(leftAsString, rightAsString, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public int GetHashCode(object obj)
            => obj.GetHashCode();
    }
}
