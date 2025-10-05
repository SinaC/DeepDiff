using DeepDiff.Configuration;
using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.CapacityAvailability;
using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Extensions;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Profiles
{
    public class CapacityAvailabilityDiffProfile : DiffProfile
    {
        public CapacityAvailabilityDiffProfile()
        {
            CreateConfiguration<Entities.CapacityAvailability.CapacityAvailability>()
                .CreateAuditEntity()
                .HasKey(x => new { x.Day, x.ContractReference })
                .HasMany(x => x.CapacityAvailabilityDetails)
                .HasOne(x => x.ForcedOutagePeriod);

            CreateConfiguration<CapacityAvailabilityDetail>()
                .AuditEntity()
                .HasKey(x => x.Direction)
                .HasValues(x => new { x.CctuNonCompliant, x.CctuNonCompliantInPreviousDays, x.WeightedCapacityPrice })
                .HasMany(x => x.CapacityAvailabilityCctus)
                .Ignore(x => new { x.ToBeRecomputed, x.CapacityAvailabilityId, x.CapacityAvailability });

            CreateConfiguration<CapacityAvailabilityCctu>()
                .AuditEntity()
                .HasKey(x => new { x.StartsOn, x.CctuName })
                .HasValues(x => new { x.AwardedVolume, x.ReservationPrice, x.Remuneration, x.IsVirtual, x.IsAllDayCctu, x.MissingVolume, x.Penalty })
                .HasMany(x => x.CapacityAvailabilityCctuDetails)
                .OnUpdate(x => x.CopyValues(y => new { y.MissingVolumeOverride, y.Status, y.SupplierStatus }))
                .Ignore(x => new { x.InternalComment, x.TsoComment, x.SupplierComment, x.CapacityAvailabilityDetailId, x.CapacityAvailabilityDetail });

            CreateConfiguration<CapacityAvailabilityCctuDetail>()
                .IdEntity()
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.AwardedVolume, x.ExchangedVolume, x.NominatedVolume, x.MissingVolume, x.ForcedOutageStatus, x.ForcedOutagePeriodStatus })
                .Ignore(x => new { x.CapacityAvailabilityCctuId, x.CapacityAvailabilityCctu });

            CreateConfiguration<ForcedOutagePeriod>()
                .IdEntity()
                .NoKey()
                .HasValues(x => new { x.FirstPeriodStart, x.FirstPeriodEnd, x.SecondPeriodStart, x.SecondPeriodEnd })
                .Ignore(x => new { x.CapacityAvailabilityId, x.CapacityAvailability });
        }
    }
}
