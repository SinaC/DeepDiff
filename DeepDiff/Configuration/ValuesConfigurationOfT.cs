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

        public IValuesConfiguration<TEntity> DisablePrecompiledEqualityComparer()
        {
            Configuration.SetUsePrecompiledEqualityComparer(false);
            return this;
        }

        public IValuesConfiguration<TEntity> EnablePrecompiledEqualityComparer()
        {
            Configuration.SetUsePrecompiledEqualityComparer(false);
            return this;
        }
    }
}