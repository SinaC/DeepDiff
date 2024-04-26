using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    public interface IEntityConfiguration<TEntity>
        where TEntity : class
    {
        IEntityConfiguration<TEntity> NoKey();

        IEntityConfiguration<TEntity> HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression);
        IEntityConfiguration<TEntity> HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression, Action<IKeyConfiguration<TEntity>> keyConfigurationAction);

        IEntityConfiguration<TEntity> HasValues<TValue>(Expression<Func<TEntity, TValue>> valuesExpression);
        IEntityConfiguration<TEntity> HasValues<TValue>(Expression<Func<TEntity, TValue>> valuesExpression, Action<IValuesConfiguration<TEntity>> valuesConfigurationAction);

        IEntityConfiguration<TEntity> HasMany<TChildEntity>(Expression<Func<TEntity, List<TChildEntity>>> navigationPropertyExpression)
            where TChildEntity : class;
        IEntityConfiguration<TEntity> HasMany<TChildEntity>(Expression<Func<TEntity, List<TChildEntity>>> navigationPropertyExpression, Action<INavigationManyConfiguration<TEntity, TChildEntity>> navigationManyConfigurationAction)
            where TChildEntity : class;

        IEntityConfiguration<TEntity> HasOne<TChildEntity>(Expression<Func<TEntity, TChildEntity>> navigationPropertyExpression)
            where TChildEntity : class;

        IEntityConfiguration<TEntity> HasOne<TChildEntity>(Expression<Func<TEntity, TChildEntity>> navigationPropertyExpression, Action<INavigationOneConfiguration<TEntity, TChildEntity>> navigationOneConfigurationAction)
            where TChildEntity : class;

        IEntityConfiguration<TEntity> OnUpdate(Action<IUpdateConfiguration<TEntity>> updateConfigurationAction);

        IEntityConfiguration<TEntity> OnInsert(Action<IInsertConfiguration<TEntity>> insertConfigurationAction);

        IEntityConfiguration<TEntity> OnDelete(Action<IDeleteConfiguration<TEntity>> deleteConfigurationAction);

        IEntityConfiguration<TEntity> WithComparer<T>(IEqualityComparer<T> equalityComparer);

        IEntityConfiguration<TEntity> WithComparer<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression, IEqualityComparer<TProperty> propertyEqualityComparer);

        IEntityConfiguration<TEntity> Ignore<TIgnore>(Expression<Func<TEntity, TIgnore>> ignoreExpression);
    }
}