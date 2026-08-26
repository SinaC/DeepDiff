namespace DeepDiff.Exceptions;

public class NoKeyAndHasKeyConfigurationException(Type entityType) : EntityConfigurationException($"HasKey and NoKey both configured for {entityType}", entityType)
{
}
