using System.Reflection;

namespace DeepDiff.Internal.Configuration;

internal sealed class ComparerConfiguration
{
    public Dictionary<Type, object> TypeSpecificComparers { get; } = []; // IEqualityComparer<T> stored as object
    public Dictionary<PropertyInfo, object> PropertySpecificComparers { get; } = []; // IEqualityComparer<T> stored as object

    public bool ContainsTypeSpecificComparer(Type propertyType)
        => TypeSpecificComparers.ContainsKey(propertyType);

    public void AddTypeSpecificComparer(Type propertyType, object equalityComparer)
    {
        TypeSpecificComparers.Add(propertyType, equalityComparer);
    }

    public bool ContainsPropertySpecificComparer(PropertyInfo propertyInfo)
        => PropertySpecificComparers.ContainsKey(propertyInfo);

    public void AddPropertySpecificComparer(PropertyInfo propertyInfo, object propertyEqualityComparer)
    {
        PropertySpecificComparers.Add(propertyInfo, propertyEqualityComparer);
    }
}
