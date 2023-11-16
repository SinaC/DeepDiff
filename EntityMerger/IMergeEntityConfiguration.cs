using System.Linq.Expressions;

namespace EntityMerger.EntityMerger;

public interface IMergeEntityConfiguration<TEntity>
    where TEntity : class
{

    IMergeEntityConfiguration<TEntity> HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression);

    IMergeEntityConfiguration<TEntity> HasCalculatedValue<TValue>(Expression<Func<TEntity, TValue>> valueExpression);

    IMergeEntityConfiguration<TEntity> HasMany<TTargetEntity>(Expression<Func<TEntity, ICollection<TTargetEntity>>> navigationPropertyExpression)
        where TTargetEntity : class;

    IMergeEntityConfiguration<TEntity> HasOne<TTargetEntity>(Expression<Func<TEntity, TTargetEntity>> navigationPropertyExpression)
        where TTargetEntity : class;

    IMergeEntityConfiguration<TEntity> MarkAsInserted<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value);

    IMergeEntityConfiguration<TEntity> MarkAsUpdated<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value);

    IMergeEntityConfiguration<TEntity> MarkAsDeleted<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value);
}