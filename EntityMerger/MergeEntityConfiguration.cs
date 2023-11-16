using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityMerger.EntityMerger;

public class MergeEntityConfiguration<TEntity> : IMergeEntityConfiguration<TEntity>
    where TEntity : class
{
    internal MergeEntityConfiguration Configuration { get; private set; }

    public MergeEntityConfiguration()
    {
        Configuration = new MergeEntityConfiguration(typeof(TEntity));
    }

    internal MergeEntityConfiguration(MergeEntityConfiguration mergeEntityConfiguration)
    {
        Configuration = mergeEntityConfiguration;
    }

    public IMergeEntityConfiguration<TEntity> HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression)
    {
        // TODO: can only be set once
        var keyProperties = keyExpression.GetSimplePropertyAccessList().Select(p => p.Single());
        var equalityComparerByProperties = new EqualityComparerByProperties<TEntity>(keyProperties);

        Configuration.SetKey(keyProperties, equalityComparerByProperties);
        return this;
    }

    public IMergeEntityConfiguration<TEntity> HasCalculatedValue<TValue>(Expression<Func<TEntity, TValue>> calculatedValueExpression)
    {
        // TODO: check if value property has not been already registered
        var calculatedValueProperties = calculatedValueExpression.GetSimplePropertyAccessList().Select(p => p.Single());
        var equalityComparerByProperties = new EqualityComparerByProperties<TEntity>(calculatedValueProperties);
        Configuration.AddCalculatedValue(calculatedValueProperties, equalityComparerByProperties);
        return this;
    }

    public IMergeEntityConfiguration<TEntity> HasMany<TTargetEntity>(Expression<Func<TEntity, ICollection<TTargetEntity>>> navigationPropertyExpression)
        where TTargetEntity : class
    {
        // TODO: must be a of type List<T>
        // TODO: check if navigation property has not been already registered
        Configuration.AddNavigationMany(navigationPropertyExpression.GetSimplePropertyAccess().Single());
        return this;
    }

    public IMergeEntityConfiguration<TEntity> HasOne<TTargetEntity>(Expression<Func<TEntity, TTargetEntity>> navigationPropertyExpression)
        where TTargetEntity : class
    {
        // TODO: check if navigation property has not been already registered
        Configuration.AddNavigationOne(navigationPropertyExpression.GetSimplePropertyAccess().Single());
        return this;
    }

    public IMergeEntityConfiguration<TEntity> MarkAsInserted<TMember>(Expression<Func<TEntity, TMember>> destinationMember,
        TMember value)
    {
        Configuration.SetMarkAsInserted(destinationMember.GetSimplePropertyAccess().Single(), value!);
        return this;
    }

    public IMergeEntityConfiguration<TEntity> MarkAsUpdated<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value)
    {
        Configuration.SetMarkAsUpdated(destinationMember.GetSimplePropertyAccess().Single(), value!);
        return this;
    }

    public IMergeEntityConfiguration<TEntity> MarkAsDeleted<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value)
    {
        Configuration.SetMarkAsDeleted(destinationMember.GetSimplePropertyAccess().Single(), value!);
        return this;
    }
}

internal class MergeEntityConfiguration
{
    public Type EntityType { get; }
    public KeyConfiguration KeyConfiguration { get; private set; }
    public CalculatedValueConfiguration CalculatedValueConfiguration { get; private set; }
    public List<PropertyInfo> NavigationManyProperties { get; private set; } = new List<PropertyInfo>();
    public List<PropertyInfo> NavigationOneProperties { get; private set; } = new List<PropertyInfo>();
    public Dictionary<MergeEntityOperation, MarkAs> MarkAsByOperation { get; private set; } = new Dictionary<MergeEntityOperation, MarkAs>();

    public MergeEntityConfiguration(Type entityType)
    {
        EntityType = entityType;
    }

    public void SetKey(IEnumerable<PropertyInfo> keyProperties, IEqualityComparer equalityComparer)
    {
        KeyConfiguration = new KeyConfiguration
        {
            KeyProperties = keyProperties.ToArray(),
            EqualityComparer = equalityComparer
        };
    }

    public void AddCalculatedValue(IEnumerable<PropertyInfo> calculatedValueProperties, IEqualityComparer equalityComparer)
    {
        CalculatedValueConfiguration = new CalculatedValueConfiguration
        {
            CalculatedValueProperties = calculatedValueProperties.ToArray(),
            EqualityComparer = equalityComparer
        };
    }

    public void AddNavigationMany(PropertyInfo navigationProperty)
    {
        NavigationManyProperties.Add(navigationProperty);
    }

    public void AddNavigationOne(PropertyInfo navigationProperty)
    {
        NavigationOneProperties.Add(navigationProperty);
    }

    public void SetMarkAsInserted(PropertyInfo destinationProperty, object value)
    {
        MarkAsByOperation[MergeEntityOperation.Insert] = new MarkAs(destinationProperty, value);
    }

    public void SetMarkAsUpdated(PropertyInfo destinationProperty, object value)
    {
        MarkAsByOperation[MergeEntityOperation.Update] = new MarkAs(destinationProperty, value);
    }

    public void SetMarkAsDeleted(PropertyInfo destinationProperty, object value)
    {
        MarkAsByOperation[MergeEntityOperation.Delete] = new MarkAs(destinationProperty, value);
    }

    internal record MarkAs
    (
        PropertyInfo DestinationProperty,
        object Value
    );
}

internal readonly struct KeyConfiguration
{
    public IReadOnlyCollection<PropertyInfo> KeyProperties { get; init; }
    public IEqualityComparer EqualityComparer { get; init; }
}

internal readonly struct CalculatedValueConfiguration
{
    public IReadOnlyCollection<PropertyInfo> CalculatedValueProperties { get; init; }
    public IEqualityComparer EqualityComparer { get; init; }
}

internal enum MergeEntityOperation
{
    None = 0,
    Insert = 1,
    Update = 2,
    Delete = 3
}
