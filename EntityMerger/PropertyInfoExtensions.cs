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

    // check if existingEntity is equal to calculatedEntity using propertyInfos
    public static bool Equals(this IEnumerable<PropertyInfo> propertyInfos, object existingEntity, object calculatedEntity)
    {
        if (propertyInfos == null)
            return false;
        foreach (var propertyInfo in propertyInfos)
        {
            var existingValue = propertyInfo.GetValue(existingEntity);
            var calculatedValue = propertyInfo.GetValue(calculatedEntity);

            if (!Equals(existingValue, calculatedValue))
                return false;
        }
        return true;
    }

    public static void CopyPropertyValues(this IEnumerable<PropertyInfo> propertyInfos, object existingEntity, object calculatedEntity)
    {
        if (propertyInfos == null)
            return;
        foreach (var propertyInfo in propertyInfos)
        {
            var calculatedValue = propertyInfo.GetValue(calculatedEntity);
            propertyInfo.SetValue(existingEntity, calculatedValue);
        }
    }
}
