using DeepDiff.Configuration;

namespace TestApp.Profile;

public class ActivationControlProfile : DiffProfile
{
    public ActivationControlProfile()
    {
        CreateConfiguration<Entities.ActivationControl.ActivationControl>()
            .AsPersistEntity()
            .HasKey(x => new { x.Day, x.ContractReference })
            .HasValues(x => new {x.TotalEnergyRequested, x.TotalDiscrepancy, x.TotalEnergyToBeSupplied, x.FailedPercentage, x.IsMeasurementExcludedCount, x.IsJumpExcludedCount})
            .OnUpdate(cfg => cfg.CopyValues(x => x.Status))
            .HasMany(x => x.ActivationControlDetails);
        CreateConfiguration<Entities.ActivationControl.ActivationControlDetail>()
            .AsPersistEntity()
            .HasKey(x => x.StartsOn)
            .HasValues(x => new { x.OfferedVolumeUp, x.OfferedVolumeDown, x.OfferedVolumeForRedispatchingUp, x.OfferedVolumeForRedispatchingDown, x.PermittedDeviationUp, x.PermittedDeviationDown, x.RampingRate, x.HasJump })
            .HasMany(x => x.TimestampDetails)
            .HasMany(x => x.DpDetails);
        CreateConfiguration<Entities.ActivationControl.ActivationControlTimestampDetail>()
            .AsPersistEntity()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergyRequested, x.EnergyRequestedForRedispatching, x.EnergySupplied, x.EnergyToBeSupplied, x.Deviation, x.PermittedDeviation, x.MaxDeviation, x.Discrepancy, x.IsJumpExcluded, x.IsMeasurementExcluded });
        CreateConfiguration<Entities.ActivationControl.ActivationControlDpDetail>()
            .AsPersistEntity()
            .HasKey(x => x.DeliveryPointEan)
            .HasValues(x => new { x.DeliveryPointName, x.Direction, x.DeliveryPointType, x.TotalEnergySupplied })
            .HasMany(x => x.TimestampDetails);
        CreateConfiguration<Entities.ActivationControl.ActivationControlDpTimestampDetail>()
            .AsPersistEntity()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergySupplied });
    }
}
