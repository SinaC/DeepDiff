using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;

namespace DeepDiff.UnitTest.Profile
{
    public class CapacityAvailabilityProfile : DiffProfile
    {
        public CapacityAvailabilityProfile()
        {
            CreateConfiguration<Entities.CapacityAvailability.CapacityAvailability>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .WithComparer(new DecimalComparer(6))
                .WithComparer(new NullableDecimalComparer(6))
                .HasKey(x => new { x.Day, x.CapacityMarketUnitId })
                .HasValues(x => x.IsEnergyContrained)
                .HasMany(x => x.CapacityAvailabilityDetails);
            CreateConfiguration<Entities.CapacityAvailability.CapacityAvailabilityDetail>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .WithComparer(new DecimalComparer(6))
                .WithComparer(new NullableDecimalComparer(6))
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.ObligatedVolume, x.AvailableVolume, x.MissingVolume })
                .OnUpdate(cfg => cfg.CopyValues(x => x.Status));
        }
    }
}
