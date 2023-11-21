using EntityMerger.Configuration;
using EntityMerger.Exceptions;
using Xunit;

namespace EntityMerger.UnitTest.Exceptions
{
    public class AlreadyDefinedPropertyExceptionTests
    {
        [Fact]
        public void Values_AlsoInKey()
        {
            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.StartsOn);

            Assert.Throws<AlreadyDefinedPropertyException>(() => mergeConfiguration.CreateMerger());
        }

        [Fact]
        public void AdditionalValuesToCopy_AlsoInKey()
        {
            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasAdditionalValuesToCopy(x => x.StartsOn);

            Assert.Throws<AlreadyDefinedPropertyException>(() => mergeConfiguration.CreateMerger());
        }

        [Fact]
        public void AdditionalValuesToCopy_AlsoInValues()
        {
            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.Penalty })
                .HasAdditionalValuesToCopy(x => x.Penalty);

            Assert.Throws<AlreadyDefinedPropertyException>(() => mergeConfiguration.CreateMerger());
        }
    }
}
