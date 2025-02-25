using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class ValuesConfiguration<TEntity> : IValuesConfiguration<TEntity>
        where TEntity : class
    {
        private ValuesConfiguration Configuration { get; }

        public ValuesConfiguration(ValuesConfiguration valuesConfiguration)
        {
            Configuration = valuesConfiguration;
        }
    }
}