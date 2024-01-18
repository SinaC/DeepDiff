using DeepDiff.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Profile
{
    public class DiffProfileTests
    {
        [Fact]
        public void AddProfile()
        {
            var entities = GenerateCapacityAvailabilities();

            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.AddProfile<CapacityAvailabilityProfile>();
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var results = deepDiff.DiffMany(entities.existingEntities, entities.newEntities).ToArray();

            Assert.Single(results.Where(x => x.PersistChange == Entities.PersistChange.Insert));
            Assert.Empty(results.Where(x => x.PersistChange == Entities.PersistChange.Update));
            Assert.Equal("CMUIDNew", results.Single(x => x.PersistChange == Entities.PersistChange.Insert).CapacityMarketUnitId);
            Assert.Equal("CMUIDExisting", results.Single(x => x.PersistChange == Entities.PersistChange.None).CapacityMarketUnitId);
        }

        [Fact]
        public void AddProfile_ForceOnUpdate()
        {
            var entities = GenerateCapacityAvailabilities();

            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.AddProfile<CapacityAvailabilityProfile>();
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var results = deepDiff.DiffMany(entities.existingEntities, entities.newEntities, cfg => cfg.ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel()).ToArray();

            Assert.Single(results.Where(x => x.PersistChange == Entities.PersistChange.Insert));
            Assert.Single(results.Where(x => x.PersistChange == Entities.PersistChange.Update));
            Assert.Equal("CMUIDNew", results.Single(x => x.PersistChange == Entities.PersistChange.Insert).CapacityMarketUnitId);
            Assert.Equal("CMUIDExisting", results.Single(x => x.PersistChange == Entities.PersistChange.Update).CapacityMarketUnitId);
        }

        // will generate 5 existing and 6 new (4 first are identical to existing, 5th top level is identical but details are different)
        private static (IEnumerable<Entities.CapacityAvailability.CapacityAvailability> existingEntities, IEnumerable<Entities.CapacityAvailability.CapacityAvailability> newEntities) GenerateCapacityAvailabilities()
        {
            var startDate = DateTime.Today;
            var dayCount = 5;
            var existingCmuId = "CMUIDExisting";
            var newCmuId = "CMUIDNew";
            var isEnergyContrained = true;

            var existingCapacityAvailabilities = Enumerable.Range(0, dayCount).Select(x => new Entities.CapacityAvailability.CapacityAvailability
            {
                Day = startDate.AddDays(x),
                CapacityMarketUnitId = existingCmuId,
                IsEnergyContrained = isEnergyContrained,
                CapacityAvailabilityDetails = GenerateDetails(startDate, Entities.CapacityAvailability.CapacityAvailabilityStatus.Validated, x, x).ToList()
            }).ToArray();
            AssignFK(existingCapacityAvailabilities, true);

            var newCapacityAvailabilities = Enumerable.Range(0, dayCount).Select(x => new Entities.CapacityAvailability.CapacityAvailability
            {
                Day = startDate.AddDays(x),
                CapacityMarketUnitId = existingCmuId,
                IsEnergyContrained = isEnergyContrained,
                CapacityAvailabilityDetails = GenerateDetails(startDate, Entities.CapacityAvailability.CapacityAvailabilityStatus.Calculated, x, x != 4 ? x : 7).ToList()
            }).Concat
            (
                new[]
                {
                new Entities.CapacityAvailability.CapacityAvailability
                {
                    Day = startDate.AddDays(1),
                    CapacityMarketUnitId = newCmuId,
                    IsEnergyContrained = isEnergyContrained,
                    CapacityAvailabilityDetails = GenerateDetails(startDate, Entities.CapacityAvailability.CapacityAvailabilityStatus.Calculated, 1, 1).ToList()
                }
                }
            ).ToArray();
            AssignFK(newCapacityAvailabilities, false);

            return (existingCapacityAvailabilities, newCapacityAvailabilities);
        }

        private static IEnumerable<Entities.CapacityAvailability.CapacityAvailabilityDetail> GenerateDetails(DateTime day, Entities.CapacityAvailability.CapacityAvailabilityStatus status, int dayShift, int calculationShift)
            => Enumerable.Range(0, 96).Select(y => GenerateDetail(day, status, dayShift, y, calculationShift));

        private static Entities.CapacityAvailability.CapacityAvailabilityDetail GenerateDetail(DateTime day, Entities.CapacityAvailability.CapacityAvailabilityStatus status, int dayShift, int tick, int calculationShift)
            => new()
            {
                StartsOn = day.AddDays(dayShift).AddMinutes(15 * tick),
                AvailableVolume = calculationShift * tick,
                MissingVolume = calculationShift * tick,
                ObligatedVolume = 2 * calculationShift + tick,
                Status = status
            };

        private static void AssignFK(IEnumerable<Entities.CapacityAvailability.CapacityAvailability> capacityAvailabilities, bool assignNavigationProperty)
        {
            foreach (var capacityAvailability in capacityAvailabilities)
            {
                foreach (var capacityAvailabilityDetail in capacityAvailability.CapacityAvailabilityDetails)
                {
                    capacityAvailabilityDetail.CapacityAvailabilityId = capacityAvailability.Id;
                    if (assignNavigationProperty)
                        capacityAvailabilityDetail.CapacityAvailability = capacityAvailability;
                }
            }
        }
    }
}
