using DeepDiff.Configuration;
using DeepDiff.Internal.Extensions;
using System.Linq.Expressions;

namespace DeepDiff.Internal.Configuration;

internal sealed class UpdateConfiguration<TEntity>(UpdateConfiguration updateConfiguration) : IUpdateConfiguration<TEntity>
    where TEntity : class
{
    private UpdateConfiguration Configuration { get; } = updateConfiguration;

    public IUpdateConfiguration<TEntity> SetValue<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember? value)
    {
        var destinationProperty = destinationMember.GetSimplePropertyAccess().Single();
        Configuration.AddSetValueConfiguration(typeof(TEntity), destinationProperty, value);
        return this;
    }

    public IUpdateConfiguration<TEntity> CopyValues<TValue>(Expression<Func<TEntity, TValue>> copyValuesExpression)
    {
        var copyValuesProperties = copyValuesExpression.GetSimplePropertyAccessList().Select(p => p.Single());
        Configuration.SetCopyValuesConfiguration(typeof(TEntity), copyValuesProperties);
        return this;
    }
}
