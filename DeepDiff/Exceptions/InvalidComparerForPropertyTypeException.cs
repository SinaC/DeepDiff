namespace DeepDiff.Exceptions;

public class InvalidComparerForPropertyTypeException(Type type) : Exception($"Comparer for {type} is not implementing IEqualityConverter<{type}>")
{
}
