namespace DeepDiff.Configuration
{
    internal sealed class ValuesConfiguration<TEntity> : IValuesConfiguration<TEntity>
        where TEntity : class
    {
        private ValuesConfiguration Configuration { get; }

        public ValuesConfiguration(ValuesConfiguration valuesConfiguration)
        {
            Configuration = valuesConfiguration;
        }

        public void DisablePrecompiledEqualityComparer()
        {
            Configuration.SetUsePrecompiledEqualityComparer(false);
        }
    }
}