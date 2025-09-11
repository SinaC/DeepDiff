using DeepDiff.Configuration;
using DeepDiff.Internal.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class UpdateConfiguration<TEntity> : IUpdateConfiguration<TEntity>
        where TEntity : class
    {
        private UpdateConfiguration Configuration { get; }

        public UpdateConfiguration(UpdateConfiguration updateConfiguration)
        {
            Configuration = updateConfiguration;
        }

        public IUpdateConfiguration<TEntity> SetValue<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember? value)
        {
            var destinationProperty = destinationMember.GetSimplePropertyAccess().Single();
            Configuration.AddSetValueConfiguration(destinationProperty, value);
            return this;
        }

        public IUpdateConfiguration<TEntity> CopyValues<TValue>(Expression<Func<TEntity, TValue>> copyValuesExpression)
        {
            var copyValuesProperties = copyValuesExpression.GetSimplePropertyAccessList().Select(p => p.Single());
            Configuration.SetCopyValuesConfiguration(copyValuesProperties);
            return this;
        }
    }
}
