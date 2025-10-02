using DeepDiff.Configuration;
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

        [Theory]
        [InlineData(EqualityComparers.Precompiled)]
        [InlineData(EqualityComparers.Naive)]
        public void AddProfile(EqualityComparers equalityComparer)
        {
            var entities = GenerateCapacityAvailabilities();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.AddProfile<CapacityAvailabilityProfile>();
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var listener = new StoreAllOperationListener();
            var results = deepDiff.MergeMany(entities.existingEntities, entities.newEntities, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
            var operations = listener.Operations;

            // missing volume is different on every qh
            // obligated volume is different on every qh
            // available volume is different on 95 qh
            Assert.Single(results, x => x.PersistChange == PersistChange.Insert);
            Assert.DoesNotContain(results, x => x.PersistChange == PersistChange.Update);
            Assert.Equal("CMUIDNew", results.Single(x => x.PersistChange == PersistChange.Insert).CapacityMarketUnitId);
            Assert.Equal("CMUIDExisting", results.Single(x => x.PersistChange == PersistChange.None).CapacityMarketUnitId);
            Assert.Equal(1 + QhCount, operations.OfType<InsertDiffOperation>().Count());
            Assert.Single(operations.OfType<InsertDiffOperation>(), x => x.EntityName == nameof(Entities.CapacityAvailability.CapacityAvailability));
            Assert.Equal(QhCount, operations.OfType<InsertDiffOperation>().Count(x => x.EntityName == nameof(CapacityAvailabilityDetail)));
            Assert.Empty(operations.OfType<DeleteDiffOperation>());
            Assert.Equal(QhCount, operations.OfType<UpdateDiffOperation>().Count(x => x.EntityName == nameof(CapacityAvailabilityDetail) && x.UpdatedProperties.Count(y => y.PropertyName == nameof(CapacityAvailabilityDetail.MissingVolume)) == 1));
            Assert.Equal(QhCount, operations.OfType<UpdateDiffOperation>().Count(x => x.EntityName == nameof(CapacityAvailabilityDetail) && x.UpdatedProperties.Count(y => y.PropertyName == nameof(CapacityAvailabilityDetail.ObligatedVolume)) == 1));
            Assert.Equal(QhCount - 1, operations.OfType<UpdateDiffOperation>().Count(x => x.EntityName == nameof(CapacityAvailabilityDetail) && x.UpdatedProperties.Count(y => y.PropertyName == nameof(CapacityAvailabilityDetail.AvailableVolume)) == 1));
            Assert.DoesNotContain(operations.OfType<UpdateDiffOperation>(), x => x.EntityName == nameof(Entities.CapacityAvailability.CapacityAvailability));
            Assert.Equal(3 * QhCount - 1, operations.OfType<UpdateDiffOperation>().Count(x => x.EntityName == nameof(CapacityAvailabilityDetail)));
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled)]
        [InlineData(EqualityComparers.Naive)]
        public void AddProfile_ForceOnUpdate(EqualityComparers equalityComparer)
        {
            var (existingEntities, newEntities) = GenerateCapacityAvailabilities();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.AddProfile<CapacityAvailabilityProfile>();
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var listener = new StoreAllOperationListener();
            var results = deepDiff.MergeMany(existingEntities, newEntities, listener, cfg => cfg.ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(true).SetEqualityComparer(equalityComparer));
            var operations = listener.Operations;

            var toto1 = operations.OfType<UpdateDiffOperation>().SelectMany(x => x.UpdatedProperties, (e, p) => new { e.EntityName, p.PropertyName }).GroupBy(x => x.PropertyName);

            // capacity availability
            //  updated forced from nested level
            // capacity availability detail
            //  missing volume is different on every qh
            //  obligated volume is different on every qh
            //  available volume is different on 95 qh
            Assert.Single(results, x => x.PersistChange == PersistChange.Insert);
            Assert.Single(results, x => x.PersistChange == PersistChange.Update);
            Assert.Equal("CMUIDNew", results.Single(x => x.PersistChange == PersistChange.Insert).CapacityMarketUnitId);
            Assert.Equal("CMUIDExisting", results.Single(x => x.PersistChange == PersistChange.Update).CapacityMarketUnitId);
            Assert.Equal(1 + QhCount, operations.OfType<InsertDiffOperation>().Count());
            Assert.Single(operations.OfType<InsertDiffOperation>(), x => x.EntityName == nameof(Entities.CapacityAvailability.CapacityAvailability));
            Assert.Equal(QhCount, operations.OfType<InsertDiffOperation>().Count(x => x.EntityName == nameof(CapacityAvailabilityDetail)));
            Assert.Empty(operations.OfType<DeleteDiffOperation>());
            Assert.DoesNotContain(operations.OfType<UpdateDiffOperation>(), x => x.EntityName == nameof(Entities.CapacityAvailability.CapacityAvailability)); // no operation on CapacityAvailability, update has been triggered from nested level
            Assert.Equal(QhCount, operations.OfType<UpdateDiffOperation>().Count(x => x.EntityName == nameof(CapacityAvailabilityDetail) && x.UpdatedProperties.Count(y => y.PropertyName == nameof(CapacityAvailabilityDetail.MissingVolume)) == 1));
            Assert.Equal(QhCount, operations.OfType<UpdateDiffOperation>().Count(x => x.EntityName == nameof(CapacityAvailabilityDetail) && x.UpdatedProperties.Count(y => y.PropertyName == nameof(CapacityAvailabilityDetail.ObligatedVolume)) == 1));
            Assert.Equal(QhCount - 1, operations.OfType<UpdateDiffOperation>().Count(x => x.EntityName == nameof(CapacityAvailabilityDetail) && x.UpdatedProperties.Count(y => y.PropertyName == nameof(CapacityAvailabilityDetail.AvailableVolume)) == 1));
            Assert.Equal(3 * QhCount - 1, operations.OfType<UpdateDiffOperation>().Count(x => x.EntityName == nameof(CapacityAvailabilityDetail))); // missing volume and obligated volume 96 times, available volume 95 times
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled)]
        [InlineData(EqualityComparers.Naive)]
        public void NoTypeSpecificComparerUsedWithProfile(EqualityComparers equalityComparer)
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
            var listener = new StoreAllOperationListener();
            var deepDiff = diffConfiguration.CreateDeepDiff();
            var result = deepDiff.MergeSingle(existing, calculated, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
            var operations = listener.Operations;

            //
            Assert.NotNull(result);
            Assert.NotEmpty(operations);
            Assert.Single(result.CapacityAvailabilityDetails);
            Assert.Equal(12.123456999m, result.CapacityAvailabilityDetails.Single().AvailableVolume);
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled)]
        [InlineData(EqualityComparers.Naive)]
        public void TypeSpecificComparerUsedWithProfile(EqualityComparers equalityComparer)
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
            var listener = new StoreAllOperationListener();
            var result = deepDiff.MergeSingle(existing, calculated, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
            var operations = listener.Operations;

            //
            Assert.Null(result);
            Assert.Empty(operations);
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
                ObligatedVolume = 2 * (1 + calculationShift) + tick,
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
