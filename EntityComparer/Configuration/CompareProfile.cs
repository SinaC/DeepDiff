using System;
using System.Collections.Generic;

namespace EntityComparer.Configuration
{
    public abstract class CompareProfile
    {
        internal Dictionary<Type, CompareEntityConfiguration> CompareEntityConfigurations { get; private set; } = new Dictionary<Type, CompareEntityConfiguration>();

        protected ICompareEntityConfiguration<TEntity> CreateCompareEntityConfiguration<TEntity>()
            where TEntity : class
        {
            var compareEntityConfiguration = new CompareEntityConfiguration(typeof(TEntity));
            CompareEntityConfigurations.Add(typeof(TEntity), compareEntityConfiguration);

            return new CompareEntityConfiguration<TEntity>(compareEntityConfiguration);
        }
    }
}
