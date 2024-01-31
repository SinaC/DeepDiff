using DeepDiff.Exceptions;
using DeepDiff.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    internal sealed class EntityConfiguration<TEntity> : IEntityConfiguration<TEntity>
        where TEntity : class
    {
        public EntityConfiguration Configuration { get; private set; }

        internal EntityConfiguration(EntityConfiguration entityConfiguration)
        {
            Configuration = entityConfiguration;
        }

        public IEntityConfiguration<TEntity> HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression)
            => HasKey(keyExpression, null);

        public IEntityConfiguration<TEntity> HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression, Action<IKeyConfiguration> keyConfigurationAction)
        {
            if (Configuration.KeyConfiguration != null)
                throw new DuplicateKeyConfigurationException(typeof(TEntity));
            var keyProperties = keyExpression.GetSimplePropertyAccessList().Select(p => p.Single());

            var config = Configuration.SetKey(keyProperties);
            keyConfigurationAction?.Invoke(config);
            return this;
        }

        public IEntityConfiguration<TEntity> HasValues<TValue>(Expression<Func<TEntity, TValue>> valuesExpression)
            => HasValues(valuesExpression, null);

        public IEntityConfiguration<TEntity> HasValues<TValue>(Expression<Func<TEntity, TValue>> valuesExpression, Action<IValuesConfiguration> valuesConfigurationAction)
        {
            if (Configuration.ValuesConfiguration != null)
                throw new DuplicateValuesConfigurationException(typeof(TEntity));
            var valueProperties = valuesExpression.GetSimplePropertyAccessList().Select(p => p.Single());
            var config = Configuration.SetValues(valueProperties);
            valuesConfigurationAction?.Invoke(config);
            return this;
        }

        public IEntityConfiguration<TEntity> HasMany<TChildEntity>(Expression<Func<TEntity, List<TChildEntity>>> navigationPropertyExpression)
            where TChildEntity : class
            => HasMany(navigationPropertyExpression, null);

        public IEntityConfiguration<TEntity> HasMany<TChildEntity>(Expression<Func<TEntity, List<TChildEntity>>> navigationPropertyExpression, Action<INavigationManyConfiguration<TEntity, TChildEntity>> navigationManyConfigurationAction)
            where TChildEntity : class
        {
            var navigationManyPropertyInfo = navigationPropertyExpression.GetSimplePropertyAccess().Single();
            var navigationManyDestinationType = typeof(TChildEntity);
            var config = Configuration.AddNavigationMany(navigationManyPropertyInfo, navigationManyDestinationType);
            var configOfT = new NavigationManyConfiguration<TEntity, TChildEntity>(config);
            navigationManyConfigurationAction?.Invoke(configOfT);
            return this;
        }

        public IEntityConfiguration<TEntity> HasOne<TChildEntity>(Expression<Func<TEntity, TChildEntity>> navigationPropertyExpression)
            where TChildEntity : class
            => HasOne(navigationPropertyExpression, null);

        public IEntityConfiguration<TEntity> HasOne<TChildEntity>(Expression<Func<TEntity, TChildEntity>> navigationPropertyExpression, Action<INavigationOneConfiguration<TEntity, TChildEntity>> navigationOneConfigurationAction)
            where TChildEntity : class
        {
            var navigationOnePropertyInfo = navigationPropertyExpression.GetSimplePropertyAccess().Single();
            var navigationOneDestinationType = typeof(TChildEntity);
            var config = Configuration.AddNavigationOne(navigationOnePropertyInfo, navigationOneDestinationType);
            var configOfT = new NavigationOneConfiguration<TEntity, TChildEntity>(config);
            navigationOneConfigurationAction?.Invoke(configOfT);
            return this;
        }

        public IEntityConfiguration<TEntity> OnUpdate(Action<IUpdateConfiguration<TEntity>> updateConfigurationAction)
        {
            var config = Configuration.GetOrSetOnUpdate();
            var configOfT = new UpdateConfiguration<TEntity>(config);
            updateConfigurationAction?.Invoke(configOfT);
            return this;
        }

        public IEntityConfiguration<TEntity> OnInsert(Action<IInsertConfiguration<TEntity>> InsertConfigurationAction)
        {
            var config = Configuration.GetOrSetOnInsert();
            var configOfT = new InsertConfiguration<TEntity>(config);
            InsertConfigurationAction?.Invoke(configOfT);
            return this;
        }

        public IEntityConfiguration<TEntity> OnDelete(Action<IDeleteConfiguration<TEntity>> DeleteConfigurationAction)
        {
            var config = Configuration.GetOrSetOnDelete();
            var configOfT = new DeleteConfiguration<TEntity>(config);
            DeleteConfigurationAction?.Invoke(configOfT);
            return this;
        }

        public IEntityConfiguration<TEntity> WithComparer<T>(IEqualityComparer<T> equalityComparer)
        {
            var propertyType = typeof(T);
            var config = Configuration.GetOrSetWithComparer();
            if (config.TypeSpecificGenericComparers.ContainsKey(propertyType) || config.TypeSpecificNonGenericComparers.ContainsKey(propertyType))
                throw new DuplicateTypeSpecificComparerConfigurationException(typeof(TEntity), propertyType);

            config.TypeSpecificNonGenericComparers.Add(propertyType, equalityComparer);
            config.TypeSpecificGenericComparers.Add(propertyType, equalityComparer);
            return this;
        }

        public IEntityConfiguration<TEntity> WithComparer<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression, IEqualityComparer<TProperty> propertyEqualityComparer)
        {
            var config = Configuration.GetOrSetWithComparer();
            var propertyInfo = propertyExpression.GetSimplePropertyAccess().Single();
            if (config.PropertySpecificGenericComparers.ContainsKey(propertyInfo) || config.PropertySpecificNonGenericComparers.ContainsKey(propertyInfo))
                throw new DuplicatePropertySpecificComparerConfigurationException(typeof(TEntity), propertyInfo);

            config.PropertySpecificNonGenericComparers.Add(propertyInfo, propertyEqualityComparer);
            config.PropertySpecificGenericComparers.Add(propertyInfo, propertyEqualityComparer);
            return this;
        }
    }
}