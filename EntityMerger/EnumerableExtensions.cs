using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EntityMerger.EntityMerger;

internal static class EnumerableExtensions
{
    public static IEnumerable<T> ConvertTo<T>(this IEnumerable items)
    {
        return items.Cast<object>().Select(x => (T)Convert.ChangeType(x, typeof(T)));
    }

    public static List<T> ConvertToList<T>(this IEnumerable items)
    {
        // see method above
        return items.ConvertTo<T>().ToList();
    }

    public static IList ConvertToList(this IEnumerable items, Type targetType)
    {
        var method = typeof(EnumerableExtensions).GetMethod(
            "ConvertToList",
            new[] { typeof(IEnumerable) });
        var generic = method.MakeGenericMethod(targetType);
        return (IList)generic.Invoke(null, new[] { items });
    }
}
