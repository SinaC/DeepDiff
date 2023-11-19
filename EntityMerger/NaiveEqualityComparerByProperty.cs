using System.Collections;
using System.Reflection;

namespace EntityMerger;

public sealed class NaiveEqualityComparerByProperty<T> : IEqualityComparer
    where T : class
{
    private IReadOnlyCollection<PropertyInfo>? Properties { get; }

    public NaiveEqualityComparerByProperty(IEnumerable<PropertyInfo> properties)
    {
        Properties = properties?.ToArray();
    }

    public new bool Equals(object? left, object? right)
    {
        if (object.ReferenceEquals(left, right))
            return true;
        if (Properties == null)
            return Equals(left, right);
        if (left is not T)
            return false;
        if (right is not T)
            return false;
        foreach (var propertyInfo in Properties)
        {
            var existingValue = propertyInfo.GetValue(left);
            var calculatedValue = propertyInfo.GetValue(right);

            if (!Equals(existingValue, calculatedValue))
                return false;
        }
        return true;
    }

    public int GetHashCode(object obj)
    {
        if (obj is not T)
            return obj.GetHashCode();
        if (Properties == null)
            return obj.GetHashCode();
        var hashCode = new HashCode();
        foreach (var propertyInfo in Properties)
        {
            var existingValue = propertyInfo.GetValue(obj);
            hashCode.Add(existingValue);
        }
        return hashCode.ToHashCode();
    }
}