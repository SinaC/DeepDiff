using EntityMerger.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EntityMerger.UnitTest.Profile
{
    public class MergeProfileTests
    {
        [Fact]
        public void AddProfile()
        {
            var entities = GenerateCapacityAvailabilities();

            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.AddProfile<CapacityAvailabilityProfile>();
            var merger = mergeConfiguration.CreateMerger();

            var results = merger.Merge(entities.existing, entities.calculated);
            Assert.Single(results);
            Assert.Equal(Entities.PersistChange.Insert, results.Single().PersistChange);
            Assert.Equal("CMUIDNew", results.Single().CapacityMarketUnitId);
        }

        [Fact]
        public void AddProfiles()
        {
            var entities = GenerateCapacityAvailabilities();

            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.AddProfiles(typeof(CapacityAvailabilityProfile).Assembly);
            var merger = mergeConfiguration.CreateMerger();

            var results = merger.Merge(entities.existing, entities.calculated);
            Assert.Single(results);
            Assert.Equal(Entities.PersistChange.Insert, results.Single().PersistChange);
            Assert.Equal("CMUIDNew", results.Single().CapacityMarketUnitId);
        }

        private (IEnumerable<Entities.CapacityAvailability.CapacityAvailability> existing, IEnumerable<Entities.CapacityAvailability.CapacityAvailability> calculated) GenerateCapacityAvailabilities()
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
                CapacityAvailabilityDetails = GenerateDetails(startDate, x).ToList()
            }).ToArray();
            AssignFK(existingCapacityAvailabilities, true);

            var calculatedCapacityAvailabilities = Enumerable.Range(0, dayCount).Select(x => new Entities.CapacityAvailability.CapacityAvailability
            {
                Day = startDate.AddDays(x),
                CapacityMarketUnitId = existingCmuId,
                IsEnergyContrained = isEnergyContrained,
                CapacityAvailabilityDetails = GenerateDetails(startDate, x).ToList()
            }).Concat
            (
                new[]
                {
                new Entities.CapacityAvailability.CapacityAvailability
                {
                    Day = startDate.AddDays(1),
                    CapacityMarketUnitId = newCmuId,
                    IsEnergyContrained = isEnergyContrained,
                    CapacityAvailabilityDetails = GenerateDetails(startDate, 1).ToList()
                }
                }
            ).ToArray();
            AssignFK(calculatedCapacityAvailabilities, false);

            return (existingCapacityAvailabilities, calculatedCapacityAvailabilities);
        }

        private static IEnumerable<Entities.CapacityAvailability.CapacityAvailabilityDetail> GenerateDetails(DateTime day, int dayShift)
            => Enumerable.Range(0, 96).Select(y => GenerateDetail(day, dayShift, y));

        private static Entities.CapacityAvailability.CapacityAvailabilityDetail GenerateDetail(DateTime day, int dayShift, int tick)
            => new()
            {
                StartsOn = day.AddDays(dayShift).AddMinutes(15 * tick),
                AvailableVolume = dayShift * tick,
                MissingVolume = dayShift * tick,
                ObligatedVolume = 2 * dayShift + tick
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
