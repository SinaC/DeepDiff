using EntityMerger.Configuration;

namespace EntityMerger.UnitTest.Profile
{
    public class CapacityAvailabilityProfile : MergeProfile
    {
        public CapacityAvailabilityProfile()
        {
            AddMergeEntityConfiguration<Entities.CapacityAvailability.CapacityAvailability>()
                .AsPersistEntity()
                .HasKey(x => new { x.Day, x.CapacityMarketUnitId })
                .HasCalculatedValue(x => x.IsEnergyContrained)
                .HasMany(x => x.CapacityAvailabilityDetails);
            AddMergeEntityConfiguration<Entities.CapacityAvailability.CapacityAvailabilityDetail>()
                .AsPersistEntity()
                .HasKey(x => x.StartsOn)
                .HasCalculatedValue(x => new { x.ObligatedVolume, x.AvailableVolume, x.MissingVolume });
        }
    }
}
