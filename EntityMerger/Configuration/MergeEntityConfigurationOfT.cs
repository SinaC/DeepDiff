using EntityMerger.Comparer;
using EntityMerger.Extensions;
using System.Linq.Expressions;

namespace EntityMerger.Configuration;

internal class MergeEntityConfiguration<TEntity> : IMergeEntityConfiguration<TEntity>
    where TEntity : class
{
    public MergeEntityConfiguration Configuration { get; private set; }

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
        SetKeyConfiguration(keyExpression);
        return this;
    }

    public IMergeEntityConfiguration<TEntity> HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression, Action<IKeyConfiguration> keyConfigurationAction)
    {
        var config = SetKeyConfiguration(keyExpression);
        keyConfigurationAction?.Invoke(config);
        return this;
    }

    private IKeyConfiguration SetKeyConfiguration<TKey>(Expression<Func<TEntity, TKey>> keyExpression)
    {
        // TODO: can only be set once
        var keyProperties = keyExpression.GetSimplePropertyAccessList().Select(p => p.Single());
        var precompiledEqualityComparerByPropertInfo = new PrecompiledEqualityComparerByProperty<TEntity>(keyProperties);
        var naiveEqualityComparerByPropertyInfo = new NaiveEqualityComparerByProperty<TEntity>(keyProperties);

        var config = Configuration.SetKey(keyProperties, precompiledEqualityComparerByPropertInfo, naiveEqualityComparerByPropertyInfo);
        return config;
    }

    public IMergeEntityConfiguration<TEntity> HasCalculatedValue<TValue>(Expression<Func<TEntity, TValue>> calculatedValueExpression)
    {
        SetCalculatedValueConfiguration(calculatedValueExpression);
        return this;
    }

    public IMergeEntityConfiguration<TEntity> HasCalculatedValue<TValue>(Expression<Func<TEntity, TValue>> calculatedValueExpression, Action<ICalculatedValueConfiguration> calculatedValueConfigurationAction)
    {
        var config = SetCalculatedValueConfiguration(calculatedValueExpression);
        calculatedValueConfigurationAction?.Invoke(config);
        return this;
    }

    private ICalculatedValueConfiguration SetCalculatedValueConfiguration<TValue>(Expression<Func<TEntity, TValue>> calculatedValueExpression)
    {
        // TODO: check if value property has not been already registered
        var calculatedValueProperties = calculatedValueExpression.GetSimplePropertyAccessList().Select(p => p.Single());
        var precompiledEqualityComparerByPropertInfo = new PrecompiledEqualityComparerByProperty<TEntity>(calculatedValueProperties);
        var naiveEqualityComparerByPropertyInfo = new NaiveEqualityComparerByProperty<TEntity>(calculatedValueProperties);
        var config = Configuration.SetCalculatedValue(calculatedValueProperties, precompiledEqualityComparerByPropertInfo, naiveEqualityComparerByPropertyInfo);
        return config;
    }

    public IMergeEntityConfiguration<TEntity> HasValueToCopy<TValue>(Expression<Func<TEntity, TValue>> valueToCopyExpression)
    {
        // TODO: check if value to ^copy property is not already set in CalculatedValue
        var valueToCopyProperties = valueToCopyExpression.GetSimplePropertyAccessList().Select(p => p.Single());
        var config = Configuration.SetValueToCopy(valueToCopyProperties);
        return this;
    }

    public IMergeEntityConfiguration<TEntity> HasMany<TTargetEntity>(Expression<Func<TEntity, List<TTargetEntity>>> navigationPropertyExpression)
        where TTargetEntity : class
    {
        var config = AddNavigationManyConfiguration(navigationPropertyExpression);
        return this;
    }

    public IMergeEntityConfiguration<TEntity> HasMany<TTargetEntity>(Expression<Func<TEntity, List<TTargetEntity>>> navigationPropertyExpression, Action<INavigationManyConfiguration> navigationManyConfigurationAction)
        where TTargetEntity : class
    {
        var config = AddNavigationManyConfiguration(navigationPropertyExpression);
        navigationManyConfigurationAction?.Invoke(config);
        return this;
    }

    private INavigationManyConfiguration AddNavigationManyConfiguration<TTargetEntity>(Expression<Func<TEntity, List<TTargetEntity>>> navigationPropertyExpression)
        where TTargetEntity : class
    {
        var navigationManyPropertyInfo = navigationPropertyExpression.GetSimplePropertyAccess().Single();
        var navigationManyDestinationType = typeof(TTargetEntity);
        var config = Configuration.AddNavigationMany(navigationManyPropertyInfo, navigationManyDestinationType);
        return config;
    }

    public IMergeEntityConfiguration<TEntity> HasOne<TTargetEntity>(Expression<Func<TEntity, TTargetEntity>> navigationPropertyExpression)
        where TTargetEntity : class
    {
        var config = Configuration.AddNavigationOne(navigationPropertyExpression.GetSimplePropertyAccess().Single());
        return this;
    }

    public IMergeEntityConfiguration<TEntity> MarkAsInserted<TMember>(Expression<Func<TEntity, TMember>> destinationMember,
        TMember value)
    {
        var config = Configuration.SetMarkAsInserted(destinationMember.GetSimplePropertyAccess().Single(), value!);
        return this;
    }

    public IMergeEntityConfiguration<TEntity> MarkAsUpdated<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value)
    {
        var config = Configuration.SetMarkAsUpdated(destinationMember.GetSimplePropertyAccess().Single(), value!);
        return this;
    }

    public IMergeEntityConfiguration<TEntity> MarkAsDeleted<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value)
    {
        var config = Configuration.SetMarkAsDeleted(destinationMember.GetSimplePropertyAccess().Single(), value!);
        return this;
    }
}