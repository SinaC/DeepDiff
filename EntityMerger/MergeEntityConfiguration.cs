using System.Collections;
using System.Reflection;

namespace EntityMerger.EntityMerger;

internal partial class MergeEntityConfiguration
{
    public Type EntityType { get; }
    public KeyConfiguration KeyConfiguration { get; private set; } = null!;
    public CalculatedValueConfiguration CalculatedValueConfiguration { get; private set; } = null!;
    public NavigationManyConfiguration NavigationManyConfiguration { get; private set; } = new NavigationManyConfiguration();
    public NavigationOneConfiguration NavigationOneConfiguration { get; private set; } = new NavigationOneConfiguration();
    public Dictionary<MergeEntityOperation, MarkAsConfiguration> MarkAsByOperation { get; private set; } = new Dictionary<MergeEntityOperation, MarkAsConfiguration>();

    public MergeEntityConfiguration(Type entityType)
    {
        EntityType = entityType;
    }

    public KeyConfiguration SetKey(IEnumerable<PropertyInfo> keyProperties, IEqualityComparer precompiledEqualityComparer, IEqualityComparer naiveEqualityComparer)
    {
        KeyConfiguration = new KeyConfiguration
        {
            KeyProperties = keyProperties.ToArray(),
            PrecompiledEqualityComparer = precompiledEqualityComparer,
            NaiveEqualityComparer = naiveEqualityComparer
        };
        return KeyConfiguration;
    }

    public CalculatedValueConfiguration SetCalculatedValue(IEnumerable<PropertyInfo> calculatedValueProperties, IEqualityComparer precompiledEqualityComparer, IEqualityComparer naiveEqualityComparer)
    {
        CalculatedValueConfiguration = new CalculatedValueConfiguration
        {
            CalculatedValueProperties = calculatedValueProperties.ToArray(),
            PrecompiledEqualityComparer = precompiledEqualityComparer,
            NaiveEqualityComparer = naiveEqualityComparer
        };
        return CalculatedValueConfiguration;
    }

    public NavigationManyConfiguration AddNavigationMany(PropertyInfo navigationProperty)
    {
        NavigationManyConfiguration.NavigationManyProperties.Add(navigationProperty);
        return NavigationManyConfiguration;
    }

    public NavigationOneConfiguration AddNavigationOne(PropertyInfo navigationProperty)
    {
        NavigationOneConfiguration.NavigationOneProperties.Add(navigationProperty);
        return NavigationOneConfiguration;
    }

    public MarkAsConfiguration SetMarkAsInserted(PropertyInfo destinationProperty, object value)
    {
        var markAsInsertedConfiguration = SetMarkAs(destinationProperty, value, MergeEntityOperation.Insert);
        return markAsInsertedConfiguration;
    }

    public MarkAsConfiguration SetMarkAsUpdated(PropertyInfo destinationProperty, object value)
    {
        var markAsUpdatedConfiguration = SetMarkAs(destinationProperty, value, MergeEntityOperation.Update);
        return markAsUpdatedConfiguration;
    }

    public MarkAsConfiguration SetMarkAsDeleted(PropertyInfo destinationProperty, object value)
    {
        var markAsDeletedConfiguration = SetMarkAs(destinationProperty, value, MergeEntityOperation.Delete);
        return markAsDeletedConfiguration;
    }

    private MarkAsConfiguration SetMarkAs(PropertyInfo destinationProperty, object value, MergeEntityOperation operation)
    {
        var markAsConfiguration = new MarkAsConfiguration
        {
            DestinationProperty = destinationProperty,
            Value = value
        };
        MarkAsByOperation[operation] = markAsConfiguration;
        return markAsConfiguration;
    }
}
