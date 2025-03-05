using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class KeyConfiguration<TEntity> : IKeyConfiguration<TEntity>
        where TEntity : class
    {
        private KeyConfiguration Configuration { get; } // cannot be removed because it will be used in the future when methods will be added IKeyConfiguration

        public KeyConfiguration(KeyConfiguration keyConfiguration)
        {
            Configuration = keyConfiguration;
        }
    }
}