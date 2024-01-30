using System;

namespace DeepDiff.Exceptions
{
    public class InvalidComparerForPropertyTypeException : Exception
    {
        public InvalidComparerForPropertyTypeException(Type type)
            : base($"Comparer for {type} is not implementing IEqualityConverter<{type}>")
        {
        }
    }
}
