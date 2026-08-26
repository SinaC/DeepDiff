using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration;

internal sealed class KeyConfiguration<TEntity>(KeyConfiguration keyConfiguration) : IKeyConfiguration<TEntity>
    where TEntity : class
{
    private KeyConfiguration Configuration { get; } = keyConfiguration;
}