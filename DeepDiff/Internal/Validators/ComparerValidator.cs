using DeepDiff.Internal.Configuration;

namespace DeepDiff.Internal.Validators;

internal sealed class ComparerValidator : ValidatorBase
{
    public override IEnumerable<Exception> Validate(Type entityType, EntityConfiguration entityConfiguration, IReadOnlyDictionary<Type, EntityConfiguration> entityConfigurationByTypes)
    {
        // no validation for the moment
        return [];
    }
}
