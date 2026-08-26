using DeepDiff.Configuration;
using DeepDiff.Internal.Extensions;
using System.Linq.Expressions;

namespace DeepDiff.Internal.Configuration;

internal sealed class InsertConfiguration<TEntity>(InsertConfiguration InsertConfiguration) : IInsertConfiguration<TEntity>
    where TEntity : class
{
    private InsertConfiguration Configuration { get; } = InsertConfiguration;

    public IInsertConfiguration<TEntity> SetValue<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember? value)
    {
        var destinationProperty = destinationMember.GetSimplePropertyAccess().Single();
        Configuration.AddSetValueConfiguration(typeof(TEntity), destinationProperty, value);
        return this;
    }
}
