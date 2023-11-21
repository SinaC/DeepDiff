using EntityMerger.Configuration;

namespace EntityMerger.UnitTest.Profile
{
    public class CapacityAvailabilityProfile : MergeProfile
    {
        public CapacityAvailabilityProfile()
        {
            CreateMergeEntityConfiguration<Entities.CapacityAvailability.CapacityAvailability>()
                .AsPersistEntity()
                .HasKey(x => new { x.Day, x.CapacityMarketUnitId })
                .HasValues(x => x.IsEnergyContrained)
                .HasMany(x => x.CapacityAvailabilityDetails);
            CreateMergeEntityConfiguration<Entities.CapacityAvailability.CapacityAvailabilityDetail>()
                .AsPersistEntity()
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.ObligatedVolume, x.AvailableVolume, x.MissingVolume })
                .HasAdditionalValuesToCopy(x => x.Status);
        }
    }
}
