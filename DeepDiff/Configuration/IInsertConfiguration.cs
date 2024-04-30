using System;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    public interface IInsertConfiguration<TEntity>
        where TEntity: class
    {
        IInsertConfiguration<TEntity> SetValue<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value);
        IInsertConfiguration<TEntity> GenerateOperations(bool generate = true);
    }
}
