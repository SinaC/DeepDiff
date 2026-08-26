using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration;

internal sealed class ValuesConfiguration<TEntity>(ValuesConfiguration valuesConfiguration) : IValuesConfiguration<TEntity>
    where TEntity : class
{
    private ValuesConfiguration Configuration { get; } = valuesConfiguration;
}