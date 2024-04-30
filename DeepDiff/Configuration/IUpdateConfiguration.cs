using System;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    public interface IUpdateConfiguration<TEntity>
        where TEntity: class
    {
        IUpdateConfiguration<TEntity> SetValue<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value);
        IUpdateConfiguration<TEntity> CopyValues<TValue>(Expression<Func<TEntity, TValue>> copyValuesExpression);
        IUpdateConfiguration<TEntity> GenerateOperations(bool generate = true);
    }
}
