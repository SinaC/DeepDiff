namespace DeepDiff.Exceptions;

public class DuplicateValuesConfigurationException(Type entityType) : EntityConfigurationException($"HasValues has already been configured for {entityType}", entityType)
{
}
