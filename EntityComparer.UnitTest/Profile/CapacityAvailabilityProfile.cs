using EntityComparer.Configuration;

namespace EntityComparer.UnitTest.Profile
{
    public class CapacityAvailabilityProfile : CompareProfile
    {
        public CapacityAvailabilityProfile()
        {
            CreateCompareEntityConfiguration<Entities.CapacityAvailability.CapacityAvailability>()
                .AsPersistEntity()
                .HasKey(x => new { x.Day, x.CapacityMarketUnitId })
                .HasValues(x => x.IsEnergyContrained)
                .HasMany(x => x.CapacityAvailabilityDetails);
            CreateCompareEntityConfiguration<Entities.CapacityAvailability.CapacityAvailabilityDetail>()
                .AsPersistEntity()
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.ObligatedVolume, x.AvailableVolume, x.MissingVolume })
                .HasAdditionalValuesToCopy(x => x.Status);
        }
    }
}
