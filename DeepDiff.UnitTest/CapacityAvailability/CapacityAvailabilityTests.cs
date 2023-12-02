using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.CapacityAvailability;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.CapacityAvailability;

public class CapacityAvailabilityTests
{
    [Fact]
    public void DetectInsertAtCapacityAvailabilityLevel()
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

        var newCapacityAvailabilities = Enumerable.Range(0, dayCount).Select(x => new Entities.CapacityAvailability.CapacityAvailability
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
        AssignFK(newCapacityAvailabilities, false);

        var deepDiff = CreateDeepDiff();
        var results = deepDiff.DiffMany(existingCapacityAvailabilities, newCapacityAvailabilities).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Insert, results.Single().PersistChange);
        Assert.Equal(newCmuId, results.Single().CapacityMarketUnitId);
    }

    private static IEnumerable<CapacityAvailabilityDetail> GenerateDetails(DateTime day, int dayShift)
        => Enumerable.Range(0, 96).Select(y => GenerateDetail(day, dayShift, y));

    private static CapacityAvailabilityDetail GenerateDetail(DateTime day, int dayShift, int tick)
        => new()
        {
            StartsOn = day.AddDays(dayShift).AddMinutes(15 * tick),
            AvailableVolume = dayShift * tick,
            MissingVolume = dayShift * tick,
            ObligatedVolume = 2 * dayShift + tick
        };

    private static void AssignFK(IEnumerable<Entities.CapacityAvailability.CapacityAvailability> capacityAvailabilities, bool assignNavigationProperty)
    {
        foreach(var capacityAvailability in capacityAvailabilities)
        {
            foreach (var capacityAvailabilityDetail in capacityAvailability.CapacityAvailabilityDetails)
            {
                capacityAvailabilityDetail.CapacityAvailabilityId = capacityAvailability.Id;
                if (assignNavigationProperty)
                    capacityAvailabilityDetail.CapacityAvailability = capacityAvailability;
            }
        }
    }

    private static IDeepDiff CreateDeepDiff()
    {
        var diffConfiguration = new DiffConfiguration();
        diffConfiguration.PersistEntity<Entities.CapacityAvailability.CapacityAvailability>()
            .HasKey(x => new { x.Day, x.CapacityMarketUnitId })
            .HasValues(x => x.IsEnergyContrained)
            .HasMany(x => x.CapacityAvailabilityDetails);
        diffConfiguration.PersistEntity<CapacityAvailabilityDetail>()
            .HasKey(x => x.StartsOn)
            .HasValues(x => new { x.ObligatedVolume, x.AvailableVolume, x.MissingVolume })
            .HasAdditionalValuesToCopy(x => x.Status);

        var deepDiff = diffConfiguration.CreateDeepDiff();
        return deepDiff;
    }
}
