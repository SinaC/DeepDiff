using System.Linq.Expressions;
using System.Reflection;

namespace EntityMerger.EntityMerger;

public class MergeEntityConfiguration<TEntityType>
    where TEntityType : PersistEntity
{
    internal MergeEntityConfiguration Configuration { get; private set; }

    public MergeEntityConfiguration()
    {
        Configuration = new MergeEntityConfiguration(typeof(TEntityType));
    }

    internal MergeEntityConfiguration(MergeEntityConfiguration mergeEntityConfiguration)
    {
        Configuration = mergeEntityConfiguration;
    }

    public MergeEntityConfiguration<TEntityType> HasKey<TKey>(Expression<Func<TEntityType, TKey>> keyExpression)
    {
        // TODO: can only be set once
        Configuration.Key(keyExpression.GetSimplePropertyAccessList().Select(p => p.Single()));
        return this;
    }

    public MergeEntityConfiguration<TEntityType> HasCalculatedValue<TValue>(Expression<Func<TEntityType, TValue>> valueExpression)
    {
        // TODO: check if value property has not been already registered
        Configuration.CalculatedValue(valueExpression.GetSimplePropertyAccessList().Select(p => p.Single()));
        return this;
    }

    public MergeEntityConfiguration<TEntityType> HasMany<TTargetEntity>(
        Expression<Func<TEntityType, ICollection<TTargetEntity>>> navigationPropertyExpression)
        where TTargetEntity : PersistEntity
    {
        // TODO: must be a of type List<T>
        // TODO: check if navigation property has not been already registered
        Configuration.Many(navigationPropertyExpression.GetSimplePropertyAccess().Single());
        return this;
    }

    public MergeEntityConfiguration<TEntityType> HasOne<TTargetEntity>(
        Expression<Func<TEntityType, TTargetEntity>> navigationPropertyExpression)
        where TTargetEntity : PersistEntity
    {
        // TODO: check if navigation property has not been already registered
        Configuration.One(navigationPropertyExpression.GetSimplePropertyAccess().Single());
        return this;
    }
}

internal class MergeEntityConfiguration
{
    public Type EntityType { get; }
    public IReadOnlyCollection<PropertyInfo> KeyProperties { get; private set; } = null!;
    public List<PropertyInfo> CalculatedValueProperties { get; private set; } = new List<PropertyInfo>();
    public List<PropertyInfo> NavigationManyProperties { get; private set; } = new List<PropertyInfo>();
    public List<PropertyInfo> NavigationOneProperties { get; private set; } = new List<PropertyInfo>();

    public MergeEntityConfiguration(Type entityType)
    {
        EntityType = entityType;
    }

    public void Key(IEnumerable<PropertyInfo> keyProperties)
    {
        KeyProperties = keyProperties.ToArray();
    }

    public void CalculatedValue(IEnumerable<PropertyInfo> valueProperties)
    {
        CalculatedValueProperties.AddRange(valueProperties);
    }

    public void Many(PropertyInfo navigationProperty)
    {
        NavigationManyProperties.Add(navigationProperty);
    }

    public void One(PropertyInfo navigationProperty)
    {
        NavigationOneProperties.Add(navigationProperty);
    }
}
