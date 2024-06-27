using DeepDiff.Configuration;
using System;
using Xunit;
using DeepDiff.Exceptions;
using System.Linq;
using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.MonthlyAggregation;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced
{
    public class ValidateIfEveryPropertiesAreReferencedTests
    {
        [Fact]
        public void ValidProfile()
        {
            var config = new DeepDiffConfiguration();
            config.AddProfile(new CapacityAvailabilityDiffProfile());
            config.AddProfile(new ActivationControlDiffProfile());
            config.AddProfile(new ActivationRemunerationDiffProfile());
            config.AddProfile(new MonthlyAggregationDiffProfile());

            config.ValidateConfiguration();
            config.ValidateIfEveryPropertiesAreReferenced();
        }

        [Fact]
        public void InvalidProfile()
        {
            var config = new DeepDiffConfiguration();
            config.AddProfile(new InvalidMonthlyAggregationDiffProfile());

            config.ValidateConfiguration();
            var ae = Assert.Throws<AggregateException>(() => config.ValidateIfEveryPropertiesAreReferenced());
            Assert.Equal(5, ae.InnerExceptions.Count);
            Assert.All(ae.InnerExceptions, x => Assert.IsType<PropertyNotReferenceInConfigurationException>(x));

            Assert.Single(ae.InnerExceptions.OfType<PropertyNotReferenceInConfigurationException>().Where(x => x.EntityType == typeof(MonthlyAggregation<MonthlyAggregationImputation>)));
            Assert.Equal(3, ae.InnerExceptions.OfType<PropertyNotReferenceInConfigurationException>().Where(x => x.EntityType == typeof(MonthlyAggregationDetail<MonthlyAggregationImputation>)).Count());
            Assert.Single(ae.InnerExceptions.OfType<PropertyNotReferenceInConfigurationException>().Where(x => x.EntityType == typeof(MonthlyAggregationImputation)));

            Assert.Single(ae.InnerExceptions.OfType<PropertyNotReferenceInConfigurationException>().Where(x => x.PropertyName == nameof(MonthlyAggregation<MonthlyAggregationImputation>.SupplierEan)));
            Assert.Single(ae.InnerExceptions.OfType<PropertyNotReferenceInConfigurationException>().Where(x => x.PropertyName == nameof(MonthlyAggregationDetail<MonthlyAggregationImputation>.Id)));
            Assert.Single(ae.InnerExceptions.OfType<PropertyNotReferenceInConfigurationException>().Where(x => x.PropertyName == nameof(MonthlyAggregationDetail<MonthlyAggregationImputation>.AuditedOn)));
            Assert.Single(ae.InnerExceptions.OfType<PropertyNotReferenceInConfigurationException>().Where(x => x.PropertyName == nameof(MonthlyAggregationDetail<MonthlyAggregationImputation>.AuditedBy)));
            Assert.Single(ae.InnerExceptions.OfType<PropertyNotReferenceInConfigurationException>().Where(x => x.PropertyName == nameof(MonthlyAggregationImputation.ValueForValidated)));
        }
    }
}
