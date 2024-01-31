namespace DeepDiff.Configuration
{
    internal sealed class KeyConfiguration<TEntity> : IKeyConfiguration<TEntity>
        where TEntity : class
    {
        public KeyConfiguration Configuration { get; }

        public KeyConfiguration(KeyConfiguration keyConfiguration)
        {
            Configuration = keyConfiguration;
        }

        public IKeyConfiguration<TEntity> DisablePrecompiledEqualityComparer()
        {
            Configuration.SetUsePrecompiledEqualityComparer(false);
            return this;
        }

        public IKeyConfiguration<TEntity> EnablePrecompiledEqualityComparer()
        {
            Configuration.SetUsePrecompiledEqualityComparer(false);
            return this;
        }
    }
}