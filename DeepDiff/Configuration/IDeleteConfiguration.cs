using System;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    public interface IDeleteConfiguration<TEntity>
        where TEntity: class
    {
        IDeleteConfiguration<TEntity> SetValue<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value);
        IDeleteConfiguration<TEntity> DisableOperationsGeneration();
        IDeleteConfiguration<TEntity> EnableOperationsGeneration();
    }
}
