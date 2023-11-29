using EntityComparer.Configuration;

namespace TestApp.Profile
{
    public class ActivationControlProfile : CompareProfile
    {
        public ActivationControlProfile()
        {
            CreateCompareEntityConfiguration<Entities.ActivationControl.ActivationControl>()
                .AsPersistEntity()
                .HasKey(x => new { x.Day, x.ContractReference })
                .HasValues(x => new {x.TotalEnergyRequested, x.TotalDiscrepancy, x.TotalEnergyToBeSupplied, x.FailedPercentage, x.IsMeasurementExcludedCount, x.IsJumpExcludedCount})
                .HasAdditionalValuesToCopy(x => x.Status)
                .HasMany(x => x.ActivationControlDetails);
            CreateCompareEntityConfiguration<Entities.ActivationControl.ActivationControlDetail>()
                .AsPersistEntity()
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.OfferedVolumeUp, x.OfferedVolumeDown, x.OfferedVolumeForRedispatchingUp, x.OfferedVolumeForRedispatchingDown, x.PermittedDeviationUp, x.PermittedDeviationDown, x.RampingRate, x.HasJump })
                .HasMany(x => x.TimestampDetails)
                .HasMany(x => x.DpDetails);
            CreateCompareEntityConfiguration<Entities.ActivationControl.ActivationControlTimestampDetail>()
                .AsPersistEntity()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergyRequested, x.EnergyRequestedForRedispatching, x.EnergySupplied, x.EnergyToBeSupplied, x.Deviation, x.PermittedDeviation, x.MaxDeviation, x.Discrepancy, x.IsJumpExcluded, x.IsMeasurementExcluded });
            CreateCompareEntityConfiguration<Entities.ActivationControl.ActivationControlDpDetail>()
                .AsPersistEntity()
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.DeliveryPointName, x.Direction, x.DeliveryPointType, x.TotalEnergySupplied })
                .HasMany(x => x.TimestampDetails);
            CreateCompareEntityConfiguration<Entities.ActivationControl.ActivationControlDpTimestampDetail>()
                .AsPersistEntity()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergySupplied });
        }
    }
}
