namespace DeepDiff.Exceptions;

public class NoKeyButFoundInNavigationManyConfigurationException(Type entityType, IEnumerable<Type> referencingEntityNavigationManyConfigurations) : EntityConfigurationException($"NoKey set to true for {entityType} but found in HasMany of {string.Join(",", referencingEntityNavigationManyConfigurations.Select(x => x.Name))}", entityType)
{
    public Type[] ReferencingEntities { get; } = referencingEntityNavigationManyConfigurations.ToArray();
}
