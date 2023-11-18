using System.Collections;
using System.Reflection;

namespace EntityMerger;

public sealed class NaiveEqualityComparerByProperties<T> : IEqualityComparer
    where T : class
{
    private IReadOnlyCollection<PropertyInfo>? PropertyInfos { get; }

    public NaiveEqualityComparerByProperties(IEnumerable<PropertyInfo> propertyInfos)
    {
        PropertyInfos = propertyInfos?.ToArray();
    }

    public new bool Equals(object? left, object? right)
    {
        if (object.ReferenceEquals(left, right))
            return true;
        if (PropertyInfos == null)
            return Equals(left, right);
        if (left is not T)
            return false;
        if (right is not T)
            return false;
        foreach (var propertyInfo in PropertyInfos)
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
        if (PropertyInfos == null)
            return obj.GetHashCode();
        var hashCode = new HashCode();
        foreach (var propertyInfo in PropertyInfos)
        {
            var existingValue = propertyInfo.GetValue(obj);
            hashCode.Add(existingValue);
        }
        return hashCode.ToHashCode();
    }
}