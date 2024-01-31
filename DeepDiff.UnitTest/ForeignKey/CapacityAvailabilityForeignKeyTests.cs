using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.CapacityAvailability;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.ForeignKey
{
    public class CapacityAvailabilityForeignKeyTests
    {
        [Fact]
        public void NoHasNavigationKey()
        {
            var deepDiff = CreateDiffWithoutNavigationKey();

            //
            var (existingEntity, newEntity) = GenerateEntities();
            var diff = deepDiff.DiffSingle(existingEntity, newEntity);
            var result = diff.Entity;

            //
            Assert.NotNull(result);
            Assert.Same(existingEntity, result); // when updated, existing is used and modified fields are copied
            Assert.Equal(PersistChange.Update, result.PersistChange);
            Assert.True(result.IsEnergyContrained); // updated field from new entity
            // 1 delete, 1 update and 1 insert
            Assert.Equal(3, result.CapacityAvailabilityDetails.Count);
            Assert.Single(result.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Delete));
            Assert.Single(result.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Update));
            Assert.Single(result.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Insert));
            // delete and update will have foreign key equals to existing entity id (were already associated to existing)
            Assert.Equal(result.Id, result.CapacityAvailabilityDetails.Single(x => x.PersistChange == PersistChange.Delete).CapacityAvailabilityId);
            Assert.Equal(result.Id, result.CapacityAvailabilityDetails.Single(x => x.PersistChange == PersistChange.Update).CapacityAvailabilityId);
            // insert will have foreign key equals to new entity id -> WRONG: should have foreign key equals to existing id
            Assert.NotEqual(result.Id, result.CapacityAvailabilityDetails.Single(x => x.PersistChange == PersistChange.Insert).CapacityAvailabilityId);
        }

        [Fact]
        public void HasNavigationKey()
        {
            var deepDiff = CreateDiffWithNavigationKey();

            //
            var (existingEntity, newEntity) = GenerateEntities();
            var diff = deepDiff.DiffSingle(existingEntity, newEntity);
            var result = diff.Entity;

            //
            Assert.NotNull(result);
            Assert.Same(existingEntity, result); // when updated, existing is used and modified fields are copied
            Assert.Equal(PersistChange.Update, result.PersistChange);
            Assert.True(result.IsEnergyContrained); // updated field from new entity
            // 1 delete, 1 update and 1 insert
            Assert.Equal(3, result.CapacityAvailabilityDetails.Count);
            Assert.Single(result.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Delete));
            Assert.Single(result.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Update));
            Assert.Single(result.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Insert));
            // check if FK are correct
            // delete, update and insert will have foreign key equals to existing entity id (were already associated to existing)
            Assert.Equal(result.Id, result.CapacityAvailabilityDetails.Single(x => x.PersistChange == PersistChange.Delete).CapacityAvailabilityId);
            Assert.Equal(result.Id, result.CapacityAvailabilityDetails.Single(x => x.PersistChange == PersistChange.Update).CapacityAvailabilityId);
            Assert.Equal(result.Id, result.CapacityAvailabilityDetails.Single(x => x.PersistChange == PersistChange.Insert).CapacityAvailabilityId);
        }

        [Fact]
        public void AddDetailWithNoExistingDetail()
        {
            var deepDiff = CreateDiffWithNavigationKey();

            //
            var (existingEntity, newEntity) = GenerateEntities();
            existingEntity.CapacityAvailabilityDetails.Clear(); // delete existing details
            var diff = deepDiff.DiffSingle(existingEntity, newEntity);
            var result = diff.Entity;

            //
            Assert.NotNull(result);
            Assert.Same(existingEntity, result); // when updated, existing is used and modified fields are copied
            Assert.Equal(PersistChange.Update, result.PersistChange);
            Assert.True(result.IsEnergyContrained); // updated field from new entity
            Assert.Equal(3, result.CapacityAvailabilityDetails.Count);
            Assert.Empty(result.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Delete));
            Assert.Empty(result.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Update));
            Assert.Equal(3, result.CapacityAvailabilityDetails.Count(x => x.PersistChange == PersistChange.Insert));
            // check if FK are correct
            Assert.All(result.CapacityAvailabilityDetails, x => Assert.Equal(result.Id, x.CapacityAvailabilityId));
        }

        [Fact]
        public void NoExisting()
        {
            var deepDiff = CreateDiffWithNavigationKey();

            //
            var (_, newEntity) = GenerateEntities();
            var diff = deepDiff.DiffSingle(null, newEntity);
            var result = diff.Entity;

            //
            Assert.NotNull(result);
            Assert.Equal(PersistChange.Insert, result.PersistChange);
            Assert.True(result.IsEnergyContrained);
            // 3 insert
            Assert.Equal(3, result.CapacityAvailabilityDetails.Count);
            Assert.Empty(result.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Delete));
            Assert.Empty(result.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Update));
            Assert.Equal(3, result.CapacityAvailabilityDetails.Count(x => x.PersistChange == PersistChange.Insert));
            // check if FK are not set
            Assert.All(result.CapacityAvailabilityDetails, x => Assert.NotEqual(result.Id, x.CapacityAvailabilityId));
        }

        [Fact]
        public void NoNew()
        {
            var deepDiff = CreateDiffWithNavigationKey();

            //
            var (existingEntity, _) = GenerateEntities();
            var diff = deepDiff.DiffSingle(existingEntity, null);
            var result = diff.Entity;

            //
            Assert.NotNull(result);
            Assert.Equal(PersistChange.Delete, result.PersistChange);
            Assert.False(result.IsEnergyContrained);
            // 3 delete
            Assert.Equal(3, result.CapacityAvailabilityDetails.Count);
            Assert.Empty(result.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Insert));
            Assert.Empty(result.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Update));
            Assert.Equal(3, result.CapacityAvailabilityDetails.Count(x => x.PersistChange == PersistChange.Delete));
            Assert.All(result.CapacityAvailabilityDetails, x => Assert.Equal(result.Id, x.CapacityAvailabilityId));
            Assert.All(result.CapacityAvailabilityDetails, x => Assert.NotEqual(x.Id, x.CapacityAvailabilityId));
        }

        private (Entities.CapacityAvailability.CapacityAvailability existing, Entities.CapacityAvailability.CapacityAvailability calculated) GenerateEntities()
        {
            var now = DateTime.Now.Date;

            var existingId = Guid.NewGuid();
            var existing = new Entities.CapacityAvailability.CapacityAvailability
            {
                Id = existingId,

                Day = now,
                CapacityMarketUnitId = "CMU1",

                IsEnergyContrained = false, // different in calculated -> update

                CapacityAvailabilityDetails = new List<CapacityAvailabilityDetail>
                {
                    new CapacityAvailabilityDetail // same in calculated -> nop
                    {
                        Id = Guid.NewGuid(),

                        StartsOn = now,

                        ObligatedVolume = 1,
                        AvailableVolume = 1,
                        MissingVolume = 1,

                        CapacityAvailabilityId = existingId,
                    },
                    new CapacityAvailabilityDetail // doesn't exist in calculated -> delete
                    {
                        Id = Guid.NewGuid(),

                        StartsOn = now.AddMinutes(15),

                        ObligatedVolume = 2,
                        AvailableVolume = 2,
                        MissingVolume = 2,

                        CapacityAvailabilityId = existingId,
                    },
                    new CapacityAvailabilityDetail // different in calculated -> update
                    {
                        Id = Guid.NewGuid(),

                        StartsOn = now.AddMinutes(30),

                        ObligatedVolume = 3,
                        AvailableVolume = 3,
                        MissingVolume = 3,

                        CapacityAvailabilityId = existingId,
                    }
                }
            };

            var calculatedId = Guid.NewGuid();
            var calculated = new Entities.CapacityAvailability.CapacityAvailability
            {
                Id = calculatedId,

                Day = now,
                CapacityMarketUnitId = "CMU1",

                IsEnergyContrained = true, // different in existing -> update

                CapacityAvailabilityDetails = new List<CapacityAvailabilityDetail>
                {
                    new CapacityAvailabilityDetail // same in existing -> nop
                    {
                        Id = Guid.NewGuid(),

                        StartsOn = now,

                        ObligatedVolume = 1,
                        AvailableVolume = 1,
                        MissingVolume = 1,
                    },
                    new CapacityAvailabilityDetail // different in existing -> update
                    {
                        Id = Guid.NewGuid(),

                        StartsOn = now.AddMinutes(30),

                        ObligatedVolume = 99,
                        AvailableVolume = 99,
                        MissingVolume = 99,
                    },
                    new CapacityAvailabilityDetail // doesn't exist in existing -> insert
                    {
                        Id = Guid.NewGuid(),

                        StartsOn = now.AddMinutes(45),

                        ObligatedVolume = 4,
                        AvailableVolume = 4,
                        MissingVolume = 4,
                    },
                }
            };

            return (existing, calculated);
        }

        private IDeepDiff CreateDiffWithoutNavigationKey()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<Entities.CapacityAvailability.CapacityAvailability>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.Day, x.CapacityMarketUnitId })
                .HasValues(x => x.IsEnergyContrained)
                .HasMany(x => x.CapacityAvailabilityDetails);
            diffConfiguration.Entity<CapacityAvailabilityDetail>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.ObligatedVolume, x.AvailableVolume, x.MissingVolume })
                .OnUpdate(cfg => cfg.CopyValues(x => x.Status));
            var deepDiff = diffConfiguration.CreateDeepDiff();
            return deepDiff;
        }

        private IDeepDiff CreateDiffWithNavigationKey()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<Entities.CapacityAvailability.CapacityAvailability>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.Day, x.CapacityMarketUnitId })
                .HasValues(x => x.IsEnergyContrained)
                .HasMany(x => x.CapacityAvailabilityDetails, cfg => cfg.HasNavigationKey(x => x.CapacityAvailabilityId, x => x.Id));
            diffConfiguration.Entity<CapacityAvailabilityDetail>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.ObligatedVolume, x.AvailableVolume, x.MissingVolume })
                .OnUpdate(cfg => cfg.CopyValues(x => x.Status));
            var deepDiff = diffConfiguration.CreateDeepDiff();
            return deepDiff;
        }
    }
}
