using DeepDiff.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    internal sealed class NavigationManyConfiguration<TEntity, TChildEntity> : INavigationManyConfiguration<TEntity, TChildEntity>
        where TEntity : class
        where TChildEntity : class
    {
        public NavigationManyConfiguration Configuration { get; private set; }

        public NavigationManyConfiguration(NavigationManyConfiguration navigationManyConfiguration)
        {
            Configuration = navigationManyConfiguration;
        }

        public INavigationManyConfiguration<TEntity, TChildEntity> HasNavigationKey<TKey>(Expression<Func<TChildEntity, TKey>> childNavigationKeyExpression, Expression<Func<TEntity, TKey>> navigationKeyExpression)
        {
            var childNavigationKeyProperty = childNavigationKeyExpression.GetSimplePropertyAccess().Single();
            var navigationKeyProperty = navigationKeyExpression.GetSimplePropertyAccess().Single();
            Configuration.AddNavigationKeyConfiguration(childNavigationKeyProperty, navigationKeyProperty);
            return this;
        }
    }
}