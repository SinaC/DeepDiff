using DeepDiff.Comparers;
using DeepDiff.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    internal sealed class DiffEntityConfiguration<TEntity> : IDiffEntityConfiguration<TEntity>
        where TEntity : class
    {
        public DiffEntityConfiguration Configuration { get; private set; }

        public DiffEntityConfiguration()
        {
            Configuration = new DiffEntityConfiguration(typeof(TEntity));
        }

        internal DiffEntityConfiguration(DiffEntityConfiguration diffEntityConfiguration)
        {
            Configuration = diffEntityConfiguration;
        }

        public IDiffEntityConfiguration<TEntity> HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression)
            => HasKey(keyExpression, null);

        public IDiffEntityConfiguration<TEntity> HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression, Action<IKeyConfiguration> keyConfigurationAction)
        {
            // TODO: can only be set once
            var keyProperties = keyExpression.GetSimplePropertyAccessList().Select(p => p.Single());
            var precompiledEqualityComparerByPropertInfo = new PrecompiledEqualityComparerByProperty<TEntity>(keyProperties);
            var naiveEqualityComparerByPropertyInfo = new NaiveEqualityComparerByProperty<TEntity>(keyProperties);

            var config = Configuration.SetKey(keyProperties, precompiledEqualityComparerByPropertInfo, naiveEqualityComparerByPropertyInfo);
            keyConfigurationAction?.Invoke(config);
            return this;
        }

        public IDiffEntityConfiguration<TEntity> HasValues<TValue>(Expression<Func<TEntity, TValue>> valuesExpression)
            => HasValues(valuesExpression, null);

        public IDiffEntityConfiguration<TEntity> HasValues<TValue>(Expression<Func<TEntity, TValue>> valuesExpression, Action<IValuesConfiguration> valuesConfigurationAction)
        {
            // TODO: can only be set once
            var valueProperties = valuesExpression.GetSimplePropertyAccessList().Select(p => p.Single());
            var precompiledEqualityComparerByPropertInfo = new PrecompiledEqualityComparerByProperty<TEntity>(valueProperties);
            var naiveEqualityComparerByPropertyInfo = new NaiveEqualityComparerByProperty<TEntity>(valueProperties);
            var config = Configuration.SetValues(valueProperties, precompiledEqualityComparerByPropertInfo, naiveEqualityComparerByPropertyInfo);
            valuesConfigurationAction?.Invoke(config);
            return this;
        }

        public IDiffEntityConfiguration<TEntity> HasMany<TTargetEntity>(Expression<Func<TEntity, List<TTargetEntity>>> navigationPropertyExpression)
            where TTargetEntity : class
            => HasMany(navigationPropertyExpression, null);

        public IDiffEntityConfiguration<TEntity> HasMany<TTargetEntity>(Expression<Func<TEntity, List<TTargetEntity>>> navigationPropertyExpression, Action<INavigationManyConfiguration> navigationManyConfigurationAction)
            where TTargetEntity : class
        {
            var navigationManyPropertyInfo = navigationPropertyExpression.GetSimplePropertyAccess().Single();
            var navigationManyDestinationType = typeof(TTargetEntity);
            var config = Configuration.AddNavigationMany(navigationManyPropertyInfo, navigationManyDestinationType);
            navigationManyConfigurationAction?.Invoke(config);
            return this;
        }

        public IDiffEntityConfiguration<TEntity> HasOne<TTargetEntity>(Expression<Func<TEntity, TTargetEntity>> navigationPropertyExpression)
            where TTargetEntity : class
            => HasOne(navigationPropertyExpression, null);

        public IDiffEntityConfiguration<TEntity> HasOne<TTargetEntity>(Expression<Func<TEntity, TTargetEntity>> navigationPropertyExpression, Action<INavigationOneConfiguration> navigationOneConfigurationAction)
            where TTargetEntity : class
        {
            var navigationOneDestinationType = typeof(TTargetEntity);
            var config = Configuration.AddNavigationOne(navigationPropertyExpression.GetSimplePropertyAccess().Single(), navigationOneDestinationType);
            navigationOneConfigurationAction?.Invoke(config);
            return this;
        }

        public IDiffEntityConfiguration<TEntity> OnUpdate(Action<IUpdateConfiguration<TEntity>> updateConfigurationAction)
        {
            var config = Configuration.GetOrSetOnUpdate();
            var configOfT = new UpdateConfiguration<TEntity>(config);
            updateConfigurationAction?.Invoke(configOfT);
            return this;
        }

        public IDiffEntityConfiguration<TEntity> OnInsert(Action<IInsertConfiguration<TEntity>> InsertConfigurationAction)
        {
            var config = Configuration.GetOrSetOnInsert();
            var configOfT = new InsertConfiguration<TEntity>(config);
            InsertConfigurationAction?.Invoke(configOfT);
            return this;
        }

        public IDiffEntityConfiguration<TEntity> OnDelete(Action<IDeleteConfiguration<TEntity>> DeleteConfigurationAction)
        {
            var config = Configuration.GetOrSetOnDelete();
            var configOfT = new DeleteConfiguration<TEntity>(config);
            DeleteConfigurationAction?.Invoke(configOfT);
            return this;
        }
    }
}