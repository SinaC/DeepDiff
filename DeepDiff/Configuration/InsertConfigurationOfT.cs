using DeepDiff.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    internal class InsertConfiguration<TEntity> : IInsertConfiguration<TEntity>
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
            var config = Configuration.SetSetValueConfiguration(destinationProperty, value);
            Configuration.SetValueConfiguration = config;
            return this;
        }
    }
}
