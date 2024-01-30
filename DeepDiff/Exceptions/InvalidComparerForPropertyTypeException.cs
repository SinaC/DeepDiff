using System;

namespace DeepDiff.Exceptions
{
    public class InvalidComparerForPropertyTypeException : Exception
    {
        public InvalidComparerForPropertyTypeException(Type type)
            : base($"Invalid comparer for {type} configured")
        {
        }
    }
}
