using DeepDiff.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    internal sealed class DeleteConfiguration<TEntity> : IDeleteConfiguration<TEntity>
        where TEntity : class
    {
        public DeleteConfiguration Configuration { get; }

        public DeleteConfiguration(DeleteConfiguration DeleteConfiguration)
        {
            Configuration = DeleteConfiguration;
        }

        public IDeleteConfiguration<TEntity> SetValue<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value)
        {
            var destinationProperty = destinationMember.GetSimplePropertyAccess().Single();
            Configuration.SetSetValueConfiguration(destinationProperty, value);
            return this;
        }

        public IDeleteConfiguration<TEntity> DisableOperationsGeneration()
        {
            Configuration.SetGenerationOperations(false);
            return this;
        }

        public IDeleteConfiguration<TEntity> EnableOperationsGeneration()
        {
            Configuration.SetGenerationOperations(true);
            return this;
        }
    }
}
