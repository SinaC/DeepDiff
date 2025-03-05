using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class ValuesConfiguration<TEntity> : IValuesConfiguration<TEntity>
        where TEntity : class
    {
        private ValuesConfiguration Configuration { get; } // cannot be removed because it will be used in the future when methods will be added in IValuesConfiguration

        public ValuesConfiguration(ValuesConfiguration valuesConfiguration)
        {
            Configuration = valuesConfiguration;
        }
    }
}