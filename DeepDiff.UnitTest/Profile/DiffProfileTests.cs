using DeepDiff.Configuration;
using DeepDiff.Operations;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.CapacityAvailability;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Profile
{
    public class DiffProfileTests
    {
        private const int QhCount = 96;

        [Fact]
        public void AddProfile()
        {
            var entities = GenerateCapacityAvailabilities();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.AddProfile<CapacityAvailabilityProfile>();
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var diff = deepDiff.DiffMany(entities.existingEntities, entities.newEntities);
            var results = diff.Entities.ToArray();
            var operations = diff.Operations;

            Assert.Single(results.Where(x => x.PersistChange == PersistChange.Insert));
            Assert.Empty(results.Where(x => x.PersistChange == PersistChange.Update));
            Assert.Equal("CMUIDNew", results.Single(x => x.PersistChange == PersistChange.Insert).CapacityMarketUnitId);
            Assert.Equal("CMUIDExisting", results.Single(x => x.PersistChange == PersistChange.None).CapacityMarketUnitId);
            Assert.Equal(1+QhCount, operations.OfType<InsertDiffOperation>().Count());
            Assert.Single(operations.OfType<InsertDiffOperation>().Where(x => x.EntityName == nameof(Entities.CapacityAvailability.CapacityAvailability)));
            Assert.Equal(QhCount, operations.OfType<InsertDiffOperation>().Count(x => x.EntityName == nameof(CapacityAvailabilityDetail)));
            Assert.Empty(operations.OfType<DeleteDiffOperation>());
            Assert.Equal(QhCount, operations.OfType<UpdateDiffOperation>().Count()); // no update on CapacityAvailability
            Assert.Empty(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(Entities.CapacityAvailability.CapacityAvailability)));
            Assert.Equal(QhCount, operations.OfType<UpdateDiffOperation>().Count(x => x.EntityName == nameof(CapacityAvailabilityDetail)));
        }

        [Fact]
        public void AddProfile_ForceOnUpdate()
        {
            var entities = GenerateCapacityAvailabilities();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.AddProfile<CapacityAvailabilityProfile>();
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var diff = deepDiff.DiffMany(entities.existingEntities, entities.newEntities, cfg => cfg.ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel());
            var results = diff.Entities.ToArray();
            var operations = diff.Operations;

            Assert.Single(results.Where(x => x.PersistChange == PersistChange.Insert));
            Assert.Single(results.Where(x => x.PersistChange == PersistChange.Update));
            Assert.Equal("CMUIDNew", results.Single(x => x.PersistChange == PersistChange.Insert).CapacityMarketUnitId);
            Assert.Equal("CMUIDExisting", results.Single(x => x.PersistChange == PersistChange.Update).CapacityMarketUnitId);
            Assert.Equal(1 + QhCount, operations.OfType<InsertDiffOperation>().Count());
            Assert.Single(operations.OfType<InsertDiffOperation>().Where(x => x.EntityName == nameof(Entities.CapacityAvailability.CapacityAvailability)));
            Assert.Equal(QhCount, operations.OfType<InsertDiffOperation>().Count(x => x.EntityName == nameof(CapacityAvailabilityDetail)));
            Assert.Empty(operations.OfType<DeleteDiffOperation>());
            Assert.Equal(1+QhCount, operations.OfType<UpdateDiffOperation>().Count());
            Assert.Single(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(Entities.CapacityAvailability.CapacityAvailability)));
            Assert.Equal(QhCount, operations.OfType<UpdateDiffOperation>().Count(x => x.EntityName == nameof(CapacityAvailabilityDetail)));
        }

        [Fact]
        public void NoTypeSpecificComparerUsedWithProfile()
        {
            var today = DateTime.Today;

            var existing = new Entities.CapacityAvailability.CapacityAvailability
            {
                Day = today,
                CapacityMarketUnitId = "CMUID",

                CapacityAvailabilityDetails = new List<CapacityAvailabilityDetail>
                {
                    new CapacityAvailabilityDetail
                    {
                        StartsOn = today,

                        AvailableVolume = 12.123456789m,
                    }
                }
            };

            var calculated = new Entities.CapacityAvailability.CapacityAvailability
            {
                Day = today,
                CapacityMarketUnitId = "CMUID",

                CapacityAvailabilityDetails = new List<CapacityAvailabilityDetail>
                {
                    new CapacityAvailabilityDetail
                    {
                        StartsOn = today,

                        AvailableVolume = 12.123456999m,
                    }
                }
            };

            //
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.AddProfile<CapacityAvailabilityProfileNoCustomComparer>();
            var deepDiff = diffConfiguration.CreateDeepDiff();
            var diff = deepDiff.DiffSingle(existing, calculated);

            //
            Assert.NotNull(diff.Entity);
            Assert.NotEmpty(diff.Operations);
            Assert.Single(diff.Entity.CapacityAvailabilityDetails);
            Assert.Equal(12.123456999m, diff.Entity.CapacityAvailabilityDetails.Single().AvailableVolume);
        }

        [Fact]
        public void TypeSpecificComparerUsedWithProfile()
        {
            var today = DateTime.Today;

            var existing = new Entities.CapacityAvailability.CapacityAvailability
            {
                Day = today,
                CapacityMarketUnitId = "CMUID",

                CapacityAvailabilityDetails = new List<CapacityAvailabilityDetail>
                {
                    new CapacityAvailabilityDetail
                    {
                        StartsOn = today,

                        AvailableVolume = 12.123456789m,
                    }
                }
            };

            var calculated = new Entities.CapacityAvailability.CapacityAvailability
            {
                Day = today,
                CapacityMarketUnitId = "CMUID",

                CapacityAvailabilityDetails = new List<CapacityAvailabilityDetail>
                {
                    new CapacityAvailabilityDetail
                    {
                        StartsOn = today,

                        AvailableVolume = 12.123456999m,
                    }
                }
            };

            //
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.AddProfile<CapacityAvailabilityProfile>();
            var deepDiff = diffConfiguration.CreateDeepDiff();
            var diff = deepDiff.DiffSingle(existing, calculated);

            //
            Assert.Null(diff.Entity);
            Assert.Empty(diff.Operations);
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
                CapacityAvailabilityDetails = GenerateDetails(startDate, CapacityAvailabilityStatus.Validated, x, x).ToList()
            }).ToArray();
            AssignFK(existingCapacityAvailabilities, true);

            var newCapacityAvailabilities = Enumerable.Range(0, dayCount).Select(x => new Entities.CapacityAvailability.CapacityAvailability
            {
                Day = startDate.AddDays(x),
                CapacityMarketUnitId = existingCmuId,
                IsEnergyContrained = isEnergyContrained,
                CapacityAvailabilityDetails = GenerateDetails(startDate, CapacityAvailabilityStatus.Calculated, x, x != 4 ? x : 7).ToList()
            }).Concat
            (
                new[]
                {
                    new Entities.CapacityAvailability.CapacityAvailability
                    {
                        Day = startDate.AddDays(1),
                        CapacityMarketUnitId = newCmuId,
                        IsEnergyContrained = isEnergyContrained,
                        CapacityAvailabilityDetails = GenerateDetails(startDate, CapacityAvailabilityStatus.Calculated, 1, 1).ToList()
                    }
                }
            ).ToArray();
            AssignFK(newCapacityAvailabilities, false);

            return (existingCapacityAvailabilities, newCapacityAvailabilities);
        }

        private static IEnumerable<CapacityAvailabilityDetail> GenerateDetails(DateTime day, CapacityAvailabilityStatus status, int dayShift, int calculationShift)
            => Enumerable.Range(0, QhCount).Select(y => GenerateDetail(day, status, dayShift, y, calculationShift));

        private static CapacityAvailabilityDetail GenerateDetail(DateTime day, CapacityAvailabilityStatus status, int dayShift, int tick, int calculationShift)
            => new()
            {
                StartsOn = day.AddDays(dayShift).AddMinutes(15 * tick),
                AvailableVolume = (1 + calculationShift) * tick,
                MissingVolume = 3 + calculationShift + tick,
                ObligatedVolume = 2 * (1+ calculationShift) + tick,
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
