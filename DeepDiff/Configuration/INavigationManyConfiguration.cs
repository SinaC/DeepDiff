using System;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    public interface INavigationManyConfiguration<TEntity, TChildEntity>
        where TEntity : class
        where TChildEntity : class
    {
        INavigationManyConfiguration<TEntity, TChildEntity> HasNavigationKey<TKey>(Expression<Func<TChildEntity, TKey>> childNavigationKeyExpression, Expression<Func<TEntity, TKey>> navigationKeyExpression);
    }
}