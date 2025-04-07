using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    /// <summary>
    /// Configuration for entities.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IEntityConfiguration<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Specify that the entity has no key.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exceptions.NoKeyAndHasKeyConfigurationException"></exception>
        IEntityConfiguration<TEntity> NoKey();

        /// <summary>
        /// Specify the key(s) for the entity. Will be used to detect insert and delete operations.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keyExpression"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exceptions.DuplicateKeyConfigurationException"></exception>
        /// <exception cref="Exceptions.NoKeyAndHasKeyConfigurationException"></exception>
        IEntityConfiguration<TEntity> HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression);

        /// <summary>
        /// Specify the key(s) for the entity. Will be used to detect insert and delete operations.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keyExpression"></param>
        /// <param name="keyConfigurationAction"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exceptions.DuplicateKeyConfigurationException"></exception>
        /// <exception cref="Exceptions.NoKeyAndHasKeyConfigurationException"></exception>
        IEntityConfiguration<TEntity> HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression, Action<IKeyConfiguration<TEntity>> keyConfigurationAction);

        /// <summary>
        /// Specify the value(s) for entity. Will be used to detect update operations.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="valuesExpression"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exceptions.DuplicateValuesConfigurationException"></exception>
        IEntityConfiguration<TEntity> HasValues<TValue>(Expression<Func<TEntity, TValue>> valuesExpression);

        /// <summary>
        /// Specify the value(s) for entity. Will be used to detect update operations.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="valuesExpression"></param>
        /// <param name="valuesConfigurationAction"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exceptions.DuplicateValuesConfigurationException"></exception>
        IEntityConfiguration<TEntity> HasValues<TValue>(Expression<Func<TEntity, TValue>> valuesExpression, Action<IValuesConfiguration<TEntity>> valuesConfigurationAction);

        /// <summary>
        /// Specify that the entity has a collection of child entities.
        /// </summary>
        /// <typeparam name="TChildEntity"></typeparam>
        /// <param name="navigationPropertyExpression"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        IEntityConfiguration<TEntity> HasMany<TChildEntity>(Expression<Func<TEntity, List<TChildEntity>>> navigationPropertyExpression)
            where TChildEntity : class;

        /// <summary>
        /// Specify that the entity has a collection of child entities.
        /// </summary>
        /// <typeparam name="TChildEntity"></typeparam>
        /// <param name="navigationPropertyExpression"></param>
        /// <param name="navigationManyConfigurationAction"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        IEntityConfiguration<TEntity> HasMany<TChildEntity>(Expression<Func<TEntity, List<TChildEntity>>> navigationPropertyExpression, Action<INavigationManyConfiguration<TEntity, TChildEntity>> navigationManyConfigurationAction)
            where TChildEntity : class;

        /// <summary>
        /// Specify that the entity has a single child entity.
        /// </summary>
        /// <typeparam name="TChildEntity"></typeparam>
        /// <param name="navigationPropertyExpression"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        IEntityConfiguration<TEntity> HasOne<TChildEntity>(Expression<Func<TEntity, TChildEntity>> navigationPropertyExpression)
            where TChildEntity : class;

        /// <summary>
        /// Specify that the entity has a single child entity.
        /// </summary>
        /// <typeparam name="TChildEntity"></typeparam>
        /// <param name="navigationPropertyExpression"></param>
        /// <param name="navigationOneConfigurationAction"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        IEntityConfiguration<TEntity> HasOne<TChildEntity>(Expression<Func<TEntity, TChildEntity>> navigationPropertyExpression, Action<INavigationOneConfiguration<TEntity, TChildEntity>> navigationOneConfigurationAction)
            where TChildEntity : class;

        /// <summary>
        /// Specify the operation(s) to perform when an update is detected.
        /// </summary>
        /// <param name="updateConfigurationAction"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        IEntityConfiguration<TEntity> OnUpdate(Action<IUpdateConfiguration<TEntity>> updateConfigurationAction);

        /// <summary>
        /// Specify the operation(s) to perform when an insert is detected.
        /// </summary>
        /// <param name="insertConfigurationAction"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        IEntityConfiguration<TEntity> OnInsert(Action<IInsertConfiguration<TEntity>> insertConfigurationAction);

        /// <summary>
        /// Specify the operation(s) to perform when a delete is detected.
        /// </summary>
        /// <param name="deleteConfigurationAction"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        IEntityConfiguration<TEntity> OnDelete(Action<IDeleteConfiguration<TEntity>> deleteConfigurationAction);

        /// <summary>
        /// Specify the comparer to use for property of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        /// <exception cref="Exceptions.DuplicateTypeSpecificComparerConfigurationException"></exception>
        IEntityConfiguration<TEntity> WithComparer<T>(IEqualityComparer<T> equalityComparer);

        /// <summary>
        /// Specify the comparer to use for property <paramref name="propertyExpression"/> of type <typeparamref name="TProperty"/>
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <param name="propertyEqualityComparer"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exceptions.DuplicatePropertySpecificComparerConfigurationException"></exception>
        IEntityConfiguration<TEntity> WithComparer<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression, IEqualityComparer<TProperty> propertyEqualityComparer);

        /// <summary>
        /// Specify that the property <paramref name="ignoreExpression"/> will not be used in compare nor merge operations. This will avoid an exception when checking configuration with ValidateIfEveryPropertiesAreReferenced
        /// </summary>
        /// <typeparam name="TIgnore"></typeparam>
        /// <param name="ignoreExpression"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        IEntityConfiguration<TEntity> Ignore<TIgnore>(Expression<Func<TEntity, TIgnore>> ignoreExpression);

        /// <summary>
        /// Defines additional criteria to force an update even if no update is detected using entity value(s).
        /// </summary>
        /// <param name="forceUpdateIfConfigurationAction"></param>
        /// <returns></returns>
        IEntityConfiguration<TEntity> ForceUpdateIf(Action<IForceUpdateIfConfiguration<TEntity>> forceUpdateIfConfigurationAction);
    }
}