using System.Linq.Expressions;

namespace EntityComparer.Configuration;

public interface ICompareEntityConfiguration<TEntity>
    where TEntity : class
{

    ICompareEntityConfiguration<TEntity> HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression);
    ICompareEntityConfiguration<TEntity> HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression, Action<IKeyConfiguration> keyConfigurationAction);

    ICompareEntityConfiguration<TEntity> HasValues<TValue>(Expression<Func<TEntity, TValue>> valuesExpression);
    ICompareEntityConfiguration<TEntity> HasValues<TValue>(Expression<Func<TEntity, TValue>> valuesExpression, Action<IValuesConfiguration> valuesConfigurationAction);

    ICompareEntityConfiguration<TEntity> HasAdditionalValuesToCopy<TValue>(Expression<Func<TEntity, TValue>> additionalValuesToCopyExpression);

    ICompareEntityConfiguration<TEntity> HasMany<TTargetEntity>(Expression<Func<TEntity, List<TTargetEntity>>> navigationPropertyExpression)
        where TTargetEntity : class;
    ICompareEntityConfiguration<TEntity> HasMany<TTargetEntity>(Expression<Func<TEntity, List<TTargetEntity>>> navigationPropertyExpression, Action<INavigationManyConfiguration> navigationManyConfigurationAction)
        where TTargetEntity : class;

    ICompareEntityConfiguration<TEntity> HasOne<TTargetEntity>(Expression<Func<TEntity, TTargetEntity>> navigationPropertyExpression)
        where TTargetEntity : class;

    ICompareEntityConfiguration<TEntity> MarkAsInserted<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value);

    ICompareEntityConfiguration<TEntity> MarkAsUpdated<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value);

    ICompareEntityConfiguration<TEntity> MarkAsDeleted<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value);
}