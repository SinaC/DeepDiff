using DeepDiff.Configuration;

namespace DeepDiff.UnitTest.Profile
{
    public class DuplicateCapacityAvailabilityProfile : DiffProfile
    {
        public DuplicateCapacityAvailabilityProfile()
        {
            CreateConfiguration<Entities.CapacityAvailability.CapacityAvailability>()
                .PersistEntity()
                .HasKey(x => new { x.Day, x.CapacityMarketUnitId })
                .HasValues(x => x.IsEnergyContrained)
                .HasMany(x => x.CapacityAvailabilityDetails);
            CreateConfiguration<Entities.CapacityAvailability.CapacityAvailabilityDetail>()
                .PersistEntity()
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.ObligatedVolume, x.AvailableVolume, x.MissingVolume })
                .OnUpdate(cfg => cfg.CopyValues(x => x.Status));

            CreateConfiguration<Entities.CapacityAvailability.CapacityAvailabilityDetail>() // duplicate
                .PersistEntity()
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.ObligatedVolume, x.AvailableVolume, x.MissingVolume })
                .OnUpdate(cfg => cfg.CopyValues(x => x.Status));
        }
    }
}
