using DeepDiff.Configuration;

namespace TestAppNet5.Profile
{
    public class ActivationControlProfile : DiffProfile
    {
        public ActivationControlProfile(IDiffConfiguration diffConfiguration)
            : base(diffConfiguration)
        {
            diffConfiguration.Entity<Entities.ActivationControl.ActivationControl>()
                .PersistEntity()
                .HasKey(x => new { x.Day, x.ContractReference })
                .HasValues(x => new {x.TotalEnergyRequested, x.TotalDiscrepancy, x.TotalEnergyToBeSupplied, x.FailedPercentage, x.IsMeasurementExcludedCount, x.IsJumpExcludedCount})
                .OnUpdate(cfg => cfg.CopyValues(x => x.Status))
                .HasMany(x => x.ActivationControlDetails);
            diffConfiguration.Entity<Entities.ActivationControl.ActivationControlDetail>()
                .PersistEntity()
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.OfferedVolumeUp, x.OfferedVolumeDown, x.OfferedVolumeForRedispatchingUp, x.OfferedVolumeForRedispatchingDown, x.PermittedDeviationUp, x.PermittedDeviationDown, x.RampingRate, x.HasJump })
                .HasMany(x => x.TimestampDetails)
                .HasMany(x => x.DpDetails);
            diffConfiguration.Entity<Entities.ActivationControl.ActivationControlTimestampDetail>()
                .PersistEntity()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergyRequested, x.EnergyRequestedForRedispatching, x.EnergySupplied, x.EnergyToBeSupplied, x.Deviation, x.PermittedDeviation, x.MaxDeviation, x.Discrepancy, x.IsJumpExcluded, x.IsMeasurementExcluded });
            diffConfiguration.Entity<Entities.ActivationControl.ActivationControlDpDetail>()
                .PersistEntity()
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.DeliveryPointName, x.Direction, x.DeliveryPointType, x.TotalEnergySupplied })
                .HasMany(x => x.TimestampDetails);
            diffConfiguration.Entity<Entities.ActivationControl.ActivationControlDpTimestampDetail>()
                .PersistEntity()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergySupplied });
        }
    }
}
