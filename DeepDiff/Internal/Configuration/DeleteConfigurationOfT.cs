using DeepDiff.Configuration;
using DeepDiff.Internal.Extensions;
using System.Linq.Expressions;

namespace DeepDiff.Internal.Configuration;

internal sealed class DeleteConfiguration<TEntity>(DeleteConfiguration DeleteConfiguration) : IDeleteConfiguration<TEntity>
    where TEntity : class
{
    private DeleteConfiguration Configuration { get; } = DeleteConfiguration;

    public IDeleteConfiguration<TEntity> SetValue<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember? value)
    {
        var destinationProperty = destinationMember.GetSimplePropertyAccess().Single();
        Configuration.AddSetValueConfiguration(typeof(TEntity), destinationProperty, value);
        return this;
    }
}
