using EntityComparer.Configuration;
using Xunit;

namespace EntityComparer.UnitTest.ActivationControl
{
    public class ActivationControlTests
    {
        [Fact]
        public void Test()
        {
            var comparer = CreateComparer();

            Assert.NotNull(comparer);
        }

        private static IEntityComparer CreateComparer()
        {
            var compareConfiguration = new CompareConfiguration();
            compareConfiguration.PersistEntity<Entities.ActivationControl.ActivationControl>()
                .HasKey(x => new { x.Day, x.ContractReference })
                .HasMany(x => x.ActivationControlDetails);
            compareConfiguration.PersistEntity<Entities.ActivationControl.ActivationControlDetail>()
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.OfferedVolumeUp, x.OfferedVolumeDown, x.OfferedVolumeForRedispatchingUp, x.OfferedVolumeForRedispatchingDown, x.PermittedDeviationUp, x.PermittedDeviationDown, x.RampingRate, x.HasJump })
                .HasMany(x => x.TimestampDetails)
                .HasMany(x => x.DpDetails);
            compareConfiguration.PersistEntity<Entities.ActivationControl.ActivationControlTimestampDetail>()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergyRequested, x.EnergyRequestedForRedispatching, x.EnergySupplied, x.EnergyToBeSupplied, x.Deviation, x.PermittedDeviation, x.MaxDeviation, x.Discrepancy, x.IsJumpExcluded, x.IsMeasurementExcluded });
            compareConfiguration.PersistEntity<Entities.ActivationControl.ActivationControlDpDetail>()
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.DeliveryPointName, x.Direction, x.DeliveryPointType, x.TotalEnergySupplied })
                .HasMany(x => x.TimestampDetails);
            compareConfiguration.PersistEntity<Entities.ActivationControl.ActivationControlDpTimestampDetail>()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergySupplied });

            var comparer = compareConfiguration.CreateComparer();
            return comparer;
        }
    }
}
