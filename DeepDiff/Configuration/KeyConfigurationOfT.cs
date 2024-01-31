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

        public void DisablePrecompiledEqualityComparer()
        {
            Configuration.SetUsePrecompiledEqualityComparer(false);
        }
    }
}