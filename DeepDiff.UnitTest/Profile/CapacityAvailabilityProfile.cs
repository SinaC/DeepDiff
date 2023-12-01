using DeepDiff.Configuration;

namespace DeepDiff.UnitTest.Profile
{
    public class CapacityAvailabilityProfile : DiffProfile
    {
        public CapacityAvailabilityProfile()
        {
            CreateDiffEntityConfiguration<Entities.CapacityAvailability.CapacityAvailability>()
                .AsPersistEntity()
                .HasKey(x => new { x.Day, x.CapacityMarketUnitId })
                .HasValues(x => x.IsEnergyContrained)
                .HasMany(x => x.CapacityAvailabilityDetails);
            CreateDiffEntityConfiguration<Entities.CapacityAvailability.CapacityAvailabilityDetail>()
                .AsPersistEntity()
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.ObligatedVolume, x.AvailableVolume, x.MissingVolume })
                .HasAdditionalValuesToCopy(x => x.Status);
        }
    }
}
