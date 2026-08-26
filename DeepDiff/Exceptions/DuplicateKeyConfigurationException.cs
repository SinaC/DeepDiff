namespace DeepDiff.Exceptions;

public class DuplicateKeyConfigurationException(Type entityType) : EntityConfigurationException($"HasKey has already been configured for {entityType}", entityType)
{
}
