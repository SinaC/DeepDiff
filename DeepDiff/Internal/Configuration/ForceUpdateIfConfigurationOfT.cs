using DeepDiff.Configuration;
using DeepDiff.Internal.Extensions;
using System.Linq.Expressions;

namespace DeepDiff.Internal.Configuration;

internal sealed class ForceUpdateIfConfiguration<TEntity>(ForceUpdateIfConfiguration configuration) : IForceUpdateIfConfiguration<TEntity>
    where TEntity : class
{
    private ForceUpdateIfConfiguration Configuration { get; } = configuration;

    public IForceUpdateIfConfiguration<TEntity> NestedEntitiesModified()
    {
        Configuration.EnableNestedEntitiesModified();
        return this;
    }

    public IForceUpdateIfConfiguration<TEntity> Equals<TMember>(Expression<Func<TEntity, TMember>> compareToMember, TMember? compareToValue)
    {
        var compareToProperty = compareToMember.GetSimplePropertyAccess().Single();
        Configuration.AddEqualsConfiguration(typeof(TEntity), compareToProperty, compareToValue);
        return this;
    }
}
