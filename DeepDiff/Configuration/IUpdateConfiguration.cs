using System.Linq.Expressions;
using System;

namespace DeepDiff.Configuration
{
    public interface IUpdateConfiguration<TEntity>
        where TEntity: class
    {
        IUpdateConfiguration<TEntity> SetValue<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value);
        IUpdateConfiguration<TEntity> CopyValues<TValue>(Expression<Func<TEntity, TValue>> copyValuesExpression);
        IUpdateConfiguration<TEntity> DisableOperationsGeneration();
    }
}
