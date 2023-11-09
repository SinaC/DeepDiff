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
        Configuration.Keys(keyExpression.GetSimplePropertyAccessList().Select(p => p.Single()));
        return this;
    }

    public MergeEntityConfiguration<TEntityType> HasValue<TValue>(Expression<Func<TEntityType, TValue>> valueExpression)
    {
        Configuration.Values(valueExpression.GetSimplePropertyAccessList().Select(p => p.Single()));
        return this;
    }

    public MergeEntityConfiguration<TEntityType> HasMany<TTargetEntity>(
        Expression<Func<TEntityType, ICollection<TTargetEntity>>> navigationPropertyExpression)
        where TTargetEntity : PersistEntity
    {
        // TODO: must be a of type List<T>
        Configuration.Many(navigationPropertyExpression.GetSimplePropertyAccess().Single());
        return this;
    }

    public MergeEntityConfiguration<TEntityType> HasOne<TTargetEntity>(
        Expression<Func<TEntityType, ICollection<TTargetEntity>>> navigationPropertyExpression)
        where TTargetEntity : PersistEntity
    {
        Configuration.One(navigationPropertyExpression.GetSimplePropertyAccess().Single());
        return this;
    }
}

internal class MergeEntityConfiguration
{
    public Type EntityType { get; }
    public IReadOnlyCollection<PropertyInfo> KeyProperties { get; private set; }
    public List<PropertyInfo> ValueProperties { get; private set; }
    public List<PropertyInfo> NavigationManyProperties { get; private set; }
    public List<PropertyInfo> NavigationOneProperties { get; private set; }

    public MergeEntityConfiguration(Type entityType)
    {
        EntityType = entityType;
    }

    public void Keys(IEnumerable<PropertyInfo> keyProperties)
    {
        KeyProperties = keyProperties.ToArray();
    }

    public void Values(IEnumerable<PropertyInfo> valueProperties)
    {
        ValueProperties = ValueProperties ?? new List<PropertyInfo>();
        ValueProperties.AddRange(valueProperties);
    }

    public void Many(PropertyInfo navigationProperty)
    {
        NavigationManyProperties = NavigationManyProperties ?? new List<PropertyInfo>();
        NavigationManyProperties.Add(navigationProperty);
    }

    public void One(PropertyInfo navigationProperty)
    {
        NavigationOneProperties = NavigationOneProperties ?? new List<PropertyInfo>();
        NavigationOneProperties.Add(navigationProperty);
    }
}
