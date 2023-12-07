using DeepDiff.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    internal sealed class DeleteConfiguration<TEntity> : IDeleteConfiguration<TEntity>
        where TEntity : class
    {
        public DeleteConfiguration Configuration { get; private set; }

        public DeleteConfiguration(DeleteConfiguration DeleteConfiguration)
        {
            Configuration = DeleteConfiguration;
        }

        public IDeleteConfiguration<TEntity> SetValue<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value)
        {
            var destinationProperty = destinationMember.GetSimplePropertyAccess().Single();
            var config = Configuration.SetSetValueConfiguration(destinationProperty, value);
            Configuration.SetValueConfiguration = config;
            return this;
        }
    }
}
