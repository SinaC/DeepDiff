using EntityMerger.Configuration;
using Xunit;

namespace EntityMerger.UnitTest.ActivationControl
{
    public class ActivationControlTests
    {
        [Fact]
        public void Test()
        {
            var merger = CreateMerger();

            Assert.NotNull(merger);
        }

        private static IMerger CreateMerger()
        {
            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.PersistEntity<Entities.ActivationControl.ActivationControl>()
                .HasKey(x => new { x.Day, x.ContractReference })
                .HasMany(x => x.ActivationControlDetails);
            mergeConfiguration.PersistEntity<Entities.ActivationControl.ActivationControlDetail>()
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.OfferedVolumeUp, x.OfferedVolumeDown, x.OfferedVolumeForRedispatchingUp, x.OfferedVolumeForRedispatchingDown, x.PermittedDeviationUp, x.PermittedDeviationDown, x.RampingRate, x.HasJump })
                .HasMany(x => x.TimestampDetails)
                .HasMany(x => x.DpDetails);
            mergeConfiguration.PersistEntity<Entities.ActivationControl.ActivationControlTimestampDetail>()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergyRequested, x.EnergyRequestedForRedispatching, x.EnergySupplied, x.EnergyToBeSupplied, x.Deviation, x.PermittedDeviation, x.MaxDeviation, x.Discrepancy, x.IsJumpExcluded, x.IsMeasurementExcluded });
            mergeConfiguration.PersistEntity<Entities.ActivationControl.ActivationControlDpDetail>()
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.DeliveryPointName, x.Direction, x.DeliveryPointType, x.TotalEnergySupplied })
                .HasMany(x => x.TimestampDetails);
            mergeConfiguration.PersistEntity<Entities.ActivationControl.ActivationControlDpTimestampDetail>()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergySupplied });

            var merger = mergeConfiguration.CreateMerger();
            return merger;
        }
    }
}
