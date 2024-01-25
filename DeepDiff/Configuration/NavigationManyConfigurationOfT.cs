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
            var childNavigationKeyProperties = childNavigationKeyExpression.GetSimplePropertyAccessList().Select(p => p.Single()).ToArray();
            var navigationKeyProperties = navigationKeyExpression.GetSimplePropertyAccessList().Select(p => p.Single()).ToArray();
            for (var i = 0; i < childNavigationKeyProperties.Length; i++) // we are sure childNavigationKeyProperties and navigationKeyProperties has the same length
            {
                var childNavigationKeyProperty = childNavigationKeyProperties[i];
                var navigationKeyProperty = navigationKeyProperties[i];
                Configuration.AddNavigationKeyConfiguration(childNavigationKeyProperty, navigationKeyProperty);
            }
            return this;
        }
    }
}