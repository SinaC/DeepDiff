using DeepDiff.Configuration;
using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.ActivationControl;
using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Extensions;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Profiles
{
    internal class ActivationControlDiffProfile : DiffProfile
    {
        public ActivationControlDiffProfile()
        {
            CreateConfiguration<Entities.ActivationControl.ActivationControl>()
                .UpdateAuditEntity<Entities.ActivationControl.ActivationControl, int>()
                .HasKey(x => new { x.Day, x.ContractReference })
                .HasValues(x => new { x.TotalEnergyRequested, x.TotalDiscrepancy, x.TotalEnergySupplied, x.TotalEnergyToBeSupplied, x.FailedPercentage, x.IsMeasurementExcludedCount, x.IsJumpExcludedCount, x.QualityFactorMissing, x.QualityFactorInvalid, x.DeactivationModeExcludedCount })
                .OnUpdate(x => x.CopyValues(y => new { y.Status, y.SupplierStatus }))
                .HasMany(x => x.ActivationControlDetails)
                .HasMany(x => x.DeactivationModePeriods)
                .Ignore(x => new { x.InternalComment, x.TsoComment, x.SupplierComment });

            CreateConfiguration<ActivationControlDetail>()
                .PersistEntity()
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.OfferedVolumeUp, x.OfferedVolumeDown, x.OfferedVolumeForRedispatchingUp, x.OfferedVolumeForRedispatchingDown, x.PermittedDeviationUp, x.PermittedDeviationDown, x.RampingRate, x.HasJump, x.DeliveryPointExcludedCount })
                .HasMany(x => x.TimestampDetails)
                .HasMany(x => x.DpDetails)
                .Ignore(x => new { x.ActivationControlId, x.ActivationControl });

            CreateConfiguration<ActivationControlDeactivationModePeriod>()
                .PersistEntity()
                .HasKey(x => new { x.PeriodStart, x.PeriodEnd })
                .Ignore(x => new { x.ActivationControlId, x.ActivationControl });

            CreateConfiguration<ActivationControlTimestampDetail>()
                .PersistEntity()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergyRequested, x.EnergyRequestedForRedispatching, x.EnergySupplied, x.EnergyToBeSupplied, x.Deviation, x.PermittedDeviation, x.MaxDeviation, x.Discrepancy, x.IsJumpExcluded, x.IsMeasurementExcluded, x.IsDeactivationModeExcluded })
                .IgnoreAudit()
                .Ignore(x => new { x.ActivationControlId, x.StartsOn, x.ActivationControlDetail });

            CreateConfiguration<ActivationControlDpDetail>()
                .PersistEntity()
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.DeliveryPointName, x.DeliveryPointType, x.Direction, x.TotalEnergySupplied })
                .HasMany(x => x.TimestampDetails)
                .Ignore(x => new { x.ActivationControlId, x.StartsOn, x.ActivationControlDetail });

            CreateConfiguration<ActivationControlDpTimestampDetail>()
                .PersistEntity()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.AvailableSec, x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergySupplied, x.QualityFactorMissing, x.QualityFactorInvalid })
                .Ignore(x => new { x.ActivationControlId, x.StartsOn, x.DeliveryPointEan, x.ActivationControlDpDetail });
        }
    }
}
