using System.Collections;
using System.Reflection;

namespace EntityMerger.Configuration;

internal class MergeEntityConfiguration
{
    public Type EntityType { get; }
    public KeyConfiguration KeyConfiguration { get; private set; } = null!;
    public CalculatedValueConfiguration CalculatedValueConfiguration { get; private set; } = null!;
    public ValueToCopyConfiguration ValueToCopyConfiguration { get; private set; } = null!;
    public IList<NavigationManyConfiguration> NavigationManyConfigurations { get; private set; } = new List<NavigationManyConfiguration>();
    public IList<NavigationOneConfiguration> NavigationOneConfigurations { get; private set; } = new List<NavigationOneConfiguration>();
    public IDictionary<MergeEntityOperation, MarkAsConfiguration> MarkAsByOperation { get; private set; } = new Dictionary<MergeEntityOperation, MarkAsConfiguration>();

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

    public ValueToCopyConfiguration SetValueToCopy(IEnumerable<PropertyInfo> copyValueProperties)
    {
        ValueToCopyConfiguration = new ValueToCopyConfiguration
        {
            CopyValueProperties = copyValueProperties.ToArray(),
        };
        return ValueToCopyConfiguration;
    }

    public NavigationManyConfiguration AddNavigationMany(PropertyInfo navigationManyProperty, Type navigationManyDestinationType)
    {
        var navigationManyConfiguration = new NavigationManyConfiguration
        {
            NavigationManyProperty = navigationManyProperty,
            NavigationManyChildType = navigationManyDestinationType
        };
        NavigationManyConfigurations.Add(navigationManyConfiguration);
        return navigationManyConfiguration;
    }

    public NavigationOneConfiguration AddNavigationOne(PropertyInfo navigationOneProperty)
    {
        var navigationOneConfiguration = new NavigationOneConfiguration
        {
            NavigationOneProperty = navigationOneProperty
        };
        NavigationOneConfigurations.Add(navigationOneConfiguration);
        return navigationOneConfiguration;
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
