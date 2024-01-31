using DeepDiff.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    internal sealed class InsertConfiguration<TEntity> : IInsertConfiguration<TEntity>
        where TEntity : class
    {
        public InsertConfiguration Configuration { get; private set; }

        public InsertConfiguration(InsertConfiguration InsertConfiguration)
        {
            Configuration = InsertConfiguration;
        }

        public IInsertConfiguration<TEntity> SetValue<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value)
        {
            var destinationProperty = destinationMember.GetSimplePropertyAccess().Single();
            Configuration.SetSetValueConfiguration(destinationProperty, value);
            return this;
        }

        public IInsertConfiguration<TEntity> DisableOperationsGeneration()
        {
            Configuration.SetGenerationOperations(false);
            return this;
        }
    }
}
