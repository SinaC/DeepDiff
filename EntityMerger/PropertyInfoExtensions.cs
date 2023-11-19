using System.Reflection;

namespace EntityMerger.EntityMerger;

internal static class PropertyInfoExtensions
{
    public static bool IsSameAs(this PropertyInfo propertyInfo, PropertyInfo otherPropertyInfo)
    {
        return (propertyInfo == otherPropertyInfo) ||
               (propertyInfo.Name == otherPropertyInfo.Name
                && (propertyInfo.DeclaringType == otherPropertyInfo.DeclaringType
                    || propertyInfo.DeclaringType.IsSubclassOf(otherPropertyInfo.DeclaringType)
                    || otherPropertyInfo.DeclaringType.IsSubclassOf(propertyInfo.DeclaringType)
                    || propertyInfo.DeclaringType.GetInterfaces().Contains(otherPropertyInfo.DeclaringType)
                    || otherPropertyInfo.DeclaringType.GetInterfaces().Contains(propertyInfo.DeclaringType)));
    }

    public static void CopyPropertyValues(this IEnumerable<PropertyInfo> properties, object existingEntity, object calculatedEntity)
    {
        if (properties == null)
            return;
        foreach (var propertyInfo in properties)
        {
            var calculatedValue = propertyInfo.GetValue(calculatedEntity);
            propertyInfo.SetValue(existingEntity, calculatedValue);
        }
    }
}
