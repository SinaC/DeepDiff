using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.ForeignKey
{
    public class SimpleEntityForeignKeyTests
    {
        // TODO: same test but with HasMany (add entity in SubEntities of root level) -> should set FK to every sub entities of added entity

        [Fact]
        public void HasOne_Insert_NoHasNavigationKey()
        {
            var existingEntities = new[]
            {
                new EntityLevel0
                {
                    Index = 1,

                    Id = Guid.NewGuid(),

                    StartsOn = DateTime.Today,
                    Direction = Direction.Up,

                    RequestedPower = 1,
                    Penalty = 1,
                }
            };

            var newEntities = new[]
            {
                new EntityLevel0
                {
                    Index = 1,

                    Id = Guid.NewGuid(),

                    StartsOn = DateTime.Today,
                    Direction = Direction.Up,

                    RequestedPower = 1,
                    Penalty = 1,

                    SubEntity = new EntityLevel1
                    {
                        Index = 1,

                        Id = Guid.NewGuid(),

                        Timestamp = DateTime.Today.AddMinutes(15),

                        Power = 1,
                        SubEntities = new List<EntityLevel2>
                        {
                            new EntityLevel2
                            {
                                Index = 1,

                                Id = Guid.NewGuid(),

                                DeliveryPointEan = "DPEAN",
                                Value1 = 1,
                            }
                        }
                    }
                }
            };

            DiffConfiguration diffConfiguration = new DiffConfiguration();
            diffConfiguration.PersistEntity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .HasOne(x => x.SubEntity);
            diffConfiguration.PersistEntity<EntityLevel1>()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.Power })
                .HasMany(x => x.SubEntities);
            diffConfiguration.PersistEntity<EntityLevel2>()
               .HasKey(x => x.DeliveryPointEan)
               .HasValues(x => new { x.Value1, x.Value2 });

            var deepDiff = diffConfiguration.CreateDeepDiff();
            var diff = deepDiff.DiffMany(existingEntities, newEntities);
            var results = diff.Entities.ToArray();

            Assert.Single(results);
            Assert.Equal(PersistChange.None, results.Single().PersistChange);
            Assert.Equal(PersistChange.Insert, results.Single().SubEntity.PersistChange);
            Assert.NotEqual(results.Single().Id, results.Single().SubEntity.EntityLevel0Id); // PK has NOT been copied to FK
            Assert.Single(results.Single().SubEntity.SubEntities);
            Assert.NotEqual(results.Single().SubEntity.Id, results.Single().SubEntity.SubEntities.Single().EntityLevel1Id); // PK has NOT been copied to FK
        }

        [Fact]
        public void HasOne_Insert_HasNavigationKey()
        {
            var existingEntities = new[]
            {
                new EntityLevel0
                {
                    Index = 1,

                    Id = Guid.NewGuid(),

                    StartsOn = DateTime.Today,
                    Direction = Direction.Up,

                    RequestedPower = 1,
                    Penalty = 1,
                }
            };

            var newEntities = new[]
            {
                new EntityLevel0
                {
                    Index = 1,

                    Id = Guid.NewGuid(),

                    StartsOn = DateTime.Today,
                    Direction = Direction.Up,

                    RequestedPower = 1,
                    Penalty = 1,

                    SubEntity = new EntityLevel1
                    {
                        Index = 1,

                        Id = Guid.NewGuid(),

                        Timestamp = DateTime.Today.AddMinutes(15),

                        Power = 1,
                        SubEntities = new List<EntityLevel2>
                        {
                            new EntityLevel2
                            {
                                Index = 1,

                                Id = Guid.NewGuid(),

                                DeliveryPointEan = "DPEAN",
                                Value1 = 1,
                            }
                        }
                    }
                }
            };

            DiffConfiguration diffConfiguration = new DiffConfiguration();
            diffConfiguration.PersistEntity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .HasOne(x => x.SubEntity, cfg => cfg.HasNavigationKey(x => x.EntityLevel0Id, x => x.Id));
            diffConfiguration.PersistEntity<EntityLevel1>()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.Power })
                .HasMany(x => x.SubEntities, cfg => cfg.HasNavigationKey(x => x.EntityLevel1Id, x => x.Id));
            diffConfiguration.PersistEntity<EntityLevel2>()
               .HasKey(x => x.DeliveryPointEan)
               .HasValues(x => new { x.Value1, x.Value2 });

            var deepDiff = diffConfiguration.CreateDeepDiff();
            var diff = deepDiff.DiffMany(existingEntities, newEntities);
            var results = diff.Entities.ToArray();

            Assert.Single(results);
            Assert.Equal(PersistChange.None, results.Single().PersistChange);
            Assert.Equal(PersistChange.Insert, results.Single().SubEntity.PersistChange);
            Assert.Equal(results.Single().Id, results.Single().SubEntity.EntityLevel0Id); // PK has been copied to FK
            Assert.Single(results.Single().SubEntity.SubEntities);
            Assert.Equal(results.Single().SubEntity.Id, results.Single().SubEntity.SubEntities.Single().EntityLevel1Id); // PK has been copied to FK
        }
    }
}
