using System.Linq.Expressions;

namespace EntityMerger.EntityMerger;

public interface IMergeEntityConfiguration<TEntityType>
    where TEntityType : class
{

    IMergeEntityConfiguration<TEntityType> HasKey<TKey>(Expression<Func<TEntityType, TKey>> keyExpression);

    IMergeEntityConfiguration<TEntityType> HasCalculatedValue<TValue>(Expression<Func<TEntityType, TValue>> valueExpression);

    IMergeEntityConfiguration<TEntityType> HasMany<TTargetEntity>(Expression<Func<TEntityType, ICollection<TTargetEntity>>> navigationPropertyExpression)
        where TTargetEntity : class;

    IMergeEntityConfiguration<TEntityType> HasOne<TTargetEntity>(Expression<Func<TEntityType, TTargetEntity>> navigationPropertyExpression)
        where TTargetEntity : class;

    IMergeEntityConfiguration<TEntityType> OnInsert<TMember>(Expression<Func<TEntityType, TMember>> destinationMember, TMember value);

    IMergeEntityConfiguration<TEntityType> OnUpdate<TMember>(Expression<Func<TEntityType, TMember>> destinationMember, TMember value);

    IMergeEntityConfiguration<TEntityType> OnDelete<TMember>(Expression<Func<TEntityType, TMember>> destinationMember, TMember value);
}