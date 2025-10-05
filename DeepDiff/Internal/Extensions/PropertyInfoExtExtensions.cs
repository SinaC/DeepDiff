using DeepDiff.Internal.Comparers;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Internal.Extensions
{
    internal static class PropertyInfoExtExtensions // following methods could be in PropertyInfoExt but for separation of concerns they are here
    {
        public static bool IsSameAs(this PropertyInfoExt propertyInfoExt, PropertyInfoExt otherPropertyInfoExt)
            => propertyInfoExt.PropertyInfo.IsSameAs(otherPropertyInfoExt.PropertyInfo);

        public static bool IsSameAs(this PropertyInfoExt propertyInfoExt, PropertyInfo otherPropertyInfo)
            => propertyInfoExt.PropertyInfo.IsSameAs(otherPropertyInfo);

        public static void CopyPropertyValues(this IEnumerable<PropertyInfoExt> propertyInfoExts, object existingEntity, object newEntity)
        {
            if (propertyInfoExts == null)
                return;
            foreach (var propertyInfoExt in propertyInfoExts)
            {
                var newValue = propertyInfoExt.GetValue(newEntity);
                propertyInfoExt.SetValue(existingEntity, newValue);
            }
        }

        public static bool IsEnumerable(this PropertyInfoExt propertyInfoExt)
        {
            if (propertyInfoExt.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyInfoExt.PropertyType))
                return true;
            return false;
        }
    }
}