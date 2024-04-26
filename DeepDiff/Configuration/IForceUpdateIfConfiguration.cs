using System;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    public interface IForceUpdateIfConfiguration<TEntity>
        where TEntity : class
    {
        IForceUpdateIfConfiguration<TEntity> NestedEntitiesModified();

        IForceUpdateIfConfiguration<TEntity> Equals<TMember>(Expression<Func<TEntity, TMember>> compareToMember, TMember compareToValue);
    }
}
