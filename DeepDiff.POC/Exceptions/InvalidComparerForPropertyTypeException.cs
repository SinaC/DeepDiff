using System;

namespace DeepDiff.POC.Exceptions
{
    public class InvalidComparerForPropertyTypeException : Exception
    {
        public InvalidComparerForPropertyTypeException(Type type)
            : base($"Comparer for {type} is not implementing IEqualityConverter<{type}>")
        {
        }
    }
}
