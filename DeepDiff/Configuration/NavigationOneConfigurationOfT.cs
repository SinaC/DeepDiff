using DeepDiff.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    internal sealed class NavigationOneConfiguration<TEntity, TChildEntity> : INavigationOneConfiguration<TEntity, TChildEntity>
        where TEntity : class
        where TChildEntity : class
    {
        public NavigationOneConfiguration Configuration { get; private set; }

        public NavigationOneConfiguration(NavigationOneConfiguration navigationOneConfiguration)
        {
            Configuration = navigationOneConfiguration;
        }

        public INavigationOneConfiguration<TEntity, TChildEntity> HasNavigationKey<TKey>(Expression<Func<TChildEntity, TKey>> childNavigationKeyExpression, Expression<Func<TEntity, TKey>> navigationKeyExpression)
        {
            var childNavigationKeyProperty = childNavigationKeyExpression.GetSimplePropertyAccess().Single();
            var navigationKeyProperty = navigationKeyExpression.GetSimplePropertyAccess().Single();
            Configuration.AddNavigationKeyConfiguration(childNavigationKeyProperty, navigationKeyProperty);
            return this;
        }
    }
}