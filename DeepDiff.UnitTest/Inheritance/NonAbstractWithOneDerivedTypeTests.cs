﻿using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Inheritance.Entities.NonAbstract;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Inheritance
{
    public class NonAbstractWithOneDerivedTypeTests
    {
        [Fact]
        public void Identical()
        {
            var existingEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntity>
                {
                    new SubEntity1
                    {
                        Key = 1001,
                        Name1 = "1001"
                    }
                }
            };

            var newEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntity>
                {
                    new SubEntity1
                    {
                        Key = 1001,
                        Name1 = "1001"
                    }
                }
            };

            var deepDiff = CreateDeepDiff();
            var diff = deepDiff.MergeSingle(existingEntity, newEntity, cfg => cfg.GenerateOperations(true));
            var result = diff.Entity;
            var operations = diff.Operations;

            Assert.Null(result);
            Assert.Empty(operations);
        }

        [Fact]
        public void InsertSubEntity()
        {
            var existingEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntity>()
            };

            var newEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntity>
                {
                    new SubEntity1
                    {
                        Key = 1002,
                        Name1 = "1002"
                    }
                }
            };

            var deepDiff = CreateDeepDiff();
            var diff = deepDiff.MergeSingle(existingEntity, newEntity, cfg => cfg.GenerateOperations(true));
            var result = diff.Entity;
            var operations = diff.Operations;

            Assert.NotNull(result);
            Assert.Single(operations);
            Assert.Equal(PersistChange.None, result.PersistChange);
            Assert.Equal(PersistChange.Insert, result.SubEntities.Single().PersistChange);
            Assert.Equal(1002, result.SubEntities.Single().Key);
        }

        [Fact]
        public void DeleteSubEntity()
        {
            var existingEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntity>
                {
                    new SubEntity1
                    {
                        Key = 1001,
                        Name1 = "1001"
                    },
                    new SubEntity1
                    {
                        Key = 1002,
                        Name1 = "1002"
                    }
                }
            };

            var newEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntity>
                {
                    new SubEntity1
                    {
                        Key = 1002,
                        Name1 = "1002"
                    }
                }
            };

            var deepDiff = CreateDeepDiff();
            var diff = deepDiff.MergeSingle(existingEntity, newEntity, cfg => cfg.GenerateOperations(true));
            var result = diff.Entity;
            var operations = diff.Operations;

            Assert.NotNull(result);
            Assert.Single(operations);
            Assert.Equal(PersistChange.None, result.PersistChange);
            Assert.Equal(PersistChange.Delete, result.SubEntities.Single().PersistChange);
            Assert.Equal(1001, result.SubEntities.Single().Key);
        }

        [Fact]
        public void UpdateSubEntity()
        {
            var existingEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntity>
                {
                    new SubEntity1
                    {
                        Key = 1001,
                        Name1 = "1001"
                    }
                }
            };

            var newEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntity>
                {
                    new SubEntity1
                    {
                        Key = 1001,
                        Name1 = "new 1001"
                    }
                }
            };

            var deepDiff = CreateDeepDiff();
            var diff = deepDiff.MergeSingle(existingEntity, newEntity, cfg => cfg.GenerateOperations(true));
            var result = diff.Entity;
            var operations = diff.Operations;

            Assert.NotNull(result);
            Assert.Single(operations);
            Assert.Equal(PersistChange.None, result.PersistChange);
            Assert.Equal(PersistChange.Update, result.SubEntities.Single().PersistChange);
            Assert.Equal(1001, result.SubEntities.Single().Key);
            Assert.Equal("new 1001", result.SubEntities.OfType<SubEntity1>().Single().Name1);
        }

        private static IDeepDiff CreateDeepDiff()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<Entity>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.Key)
                .HasValues(x => x.Name)
                .HasMany(x => x.SubEntities, opt => opt.UseDerivedTypes(true));
            diffConfiguration.Entity<SubEntity1>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.Key)
                .HasValues(x => x.Name1);

            return diffConfiguration.CreateDeepDiff();
        }
    }
}