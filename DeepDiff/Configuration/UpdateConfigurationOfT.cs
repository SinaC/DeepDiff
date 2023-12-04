using DeepDiff.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    internal class UpdateConfiguration<TEntity> : IUpdateConfiguration<TEntity>
        where TEntity : class
    {
        public UpdateConfiguration Configuration { get; private set; }

        public UpdateConfiguration(UpdateConfiguration updateConfiguration)
        {
            Configuration = updateConfiguration;
        }

        public IUpdateConfiguration<TEntity> SetValue<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value)
        {
            var destinationProperty = destinationMember.GetSimplePropertyAccess().Single();
            var config = Configuration.SetSetValueConfiguration(destinationProperty, value);
            Configuration.SetValueConfiguration = config;
            return this;
        }

        public IUpdateConfiguration<TEntity> CopyValues<TValue>(Expression<Func<TEntity, TValue>> copyValuesExpression)
        {
            var copyValuesProperties = copyValuesExpression.GetSimplePropertyAccessList().Select(p => p.Single());
            var config = Configuration.SetCopyValuesConfiguration(copyValuesProperties);
            Configuration.CopyValuesConfiguration = config;
            return this;
        }
    }
}
