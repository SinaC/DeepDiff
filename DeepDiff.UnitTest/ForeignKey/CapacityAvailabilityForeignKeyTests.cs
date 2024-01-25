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
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.PersistEntity<Entities.CapacityAvailability.CapacityAvailability>()
                .HasKey(x => new { x.Day, x.CapacityMarketUnitId })
                .HasValues(x => x.IsEnergyContrained)
                .HasMany(x => x.CapacityAvailabilityDetails);
            diffConfiguration.PersistEntity<CapacityAvailabilityDetail>()
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.ObligatedVolume, x.AvailableVolume, x.MissingVolume })
                .OnUpdate(cfg => cfg.CopyValues(x => x.Status));
            var deepDiff = diffConfiguration.CreateDeepDiff();

            //
            var (existingEntity, newEntity) = GenerateEntities();
            var diff = deepDiff.DiffSingle(existingEntity, newEntity);
            var result = diff.Entity;

            //
            Assert.NotNull(result);
            Assert.Same(existingEntity, result); // when updated, existing is used and modified fields are copied
            Assert.Equal(PersistChange.Update, existingEntity.PersistChange);
            Assert.True(existingEntity.IsEnergyContrained); // updated field from new entity
            // 1 delete, 1 update and 1 insert
            Assert.Equal(3, existingEntity.CapacityAvailabilityDetails.Count);
            Assert.Single(existingEntity.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Delete));
            Assert.Single(existingEntity.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Update));
            Assert.Single(existingEntity.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Insert));
            // delete and update will have foreign key equals to existing entity id (were already associated to existing)
            Assert.Equal(result.Id, existingEntity.CapacityAvailabilityDetails.Single(x => x.PersistChange == PersistChange.Delete).CapacityAvailabilityId);
            Assert.Equal(result.Id, existingEntity.CapacityAvailabilityDetails.Single(x => x.PersistChange == PersistChange.Update).CapacityAvailabilityId);
            // insert will have foreign key equals to new entity id -> WRONG which is wrong, should have foreign key equals to existing id
            Assert.NotEqual(result.Id, existingEntity.CapacityAvailabilityDetails.Single(x => x.PersistChange == PersistChange.Insert).CapacityAvailabilityId);
        }

        [Fact]
        public void HasNavigationKey()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.PersistEntity<Entities.CapacityAvailability.CapacityAvailability>()
                .HasKey(x => new { x.Day, x.CapacityMarketUnitId })
                .HasValues(x => x.IsEnergyContrained)
                .HasMany(x => x.CapacityAvailabilityDetails, cfg => cfg.HasNavigationKey(x => x.CapacityAvailabilityId, x => x.Id));
            diffConfiguration.PersistEntity<CapacityAvailabilityDetail>()
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.ObligatedVolume, x.AvailableVolume, x.MissingVolume })
                .OnUpdate(cfg => cfg.CopyValues(x => x.Status));
            var deepDiff = diffConfiguration.CreateDeepDiff();

            //
            var (existingEntity, newEntity) = GenerateEntities();
            var diff = deepDiff.DiffSingle(existingEntity, newEntity);
            var result = diff.Entity;

            //
            Assert.NotNull(result);
            Assert.Same(existingEntity, result); // when updated, existing is used and modified fields are copied
            Assert.Equal(PersistChange.Update, existingEntity.PersistChange);
            Assert.True(existingEntity.IsEnergyContrained); // updated field from new entity
            // 1 delete, 1 update and 1 insert
            Assert.Equal(3, existingEntity.CapacityAvailabilityDetails.Count);
            Assert.Single(existingEntity.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Delete));
            Assert.Single(existingEntity.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Update));
            Assert.Single(existingEntity.CapacityAvailabilityDetails.Where(x => x.PersistChange == PersistChange.Insert));
            // delete, update and insert will have foreign key equals to existing entity id (were already associated to existing)
            Assert.Equal(result.Id, existingEntity.CapacityAvailabilityDetails.Single(x => x.PersistChange == PersistChange.Delete).CapacityAvailabilityId);
            Assert.Equal(result.Id, existingEntity.CapacityAvailabilityDetails.Single(x => x.PersistChange == PersistChange.Update).CapacityAvailabilityId);
            Assert.Equal(result.Id, existingEntity.CapacityAvailabilityDetails.Single(x => x.PersistChange == PersistChange.Insert).CapacityAvailabilityId);
        }

        private (Entities.CapacityAvailability.CapacityAvailability existing, Entities.CapacityAvailability.CapacityAvailability calculated) GenerateEntities()
        {
            var now = DateTime.Now;

            var existingId = Guid.NewGuid();
            var existing = new Entities.CapacityAvailability.CapacityAvailability
            {
                Id = existingId,

                Day = now.Date,
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

                Day = now.Date,
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

                        CapacityAvailabilityId = calculatedId,
                    },
                    new CapacityAvailabilityDetail // different in existing -> update
                    {
                        Id = Guid.NewGuid(),

                        StartsOn = now.AddMinutes(30),

                        ObligatedVolume = 99,
                        AvailableVolume = 99,
                        MissingVolume = 99,

                        CapacityAvailabilityId = calculatedId,
                    },
                    new CapacityAvailabilityDetail // doesn't exist in existing -> insert
                    {
                        Id = Guid.NewGuid(),

                        StartsOn = now.AddMinutes(45),

                        ObligatedVolume = 4,
                        AvailableVolume = 4,
                        MissingVolume = 4,

                        CapacityAvailabilityId = calculatedId,
                    },
                }
            };

            return (existing, calculated);
        }
    }
}
