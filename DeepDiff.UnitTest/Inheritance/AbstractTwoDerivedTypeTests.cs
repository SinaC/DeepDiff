﻿using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Inheritance.Entities.Abstract;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Inheritance
{
    public class AbstractTwoDerivedTypeTests
    {
        [Theory]
        [InlineData(EqualityComparers.Precompiled)]
        [InlineData(EqualityComparers.Naive)]
        public void Identical(EqualityComparers equalityComparer)
        {
            var existingEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntityBase>
                {
                    new SubEntity1
                    {
                        Key = 11,
                        Name1 = "11"
                    },
                    new SubEntity2
                    {
                        Key = 21,
                        Name2 = "21"
                    }
                }
            };

            var newEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntityBase>
                {
                    new SubEntity1
                    {
                        Key = 11,
                        Name1 = "11"
                    },
                    new SubEntity2
                    {
                        Key = 21,
                        Name2 = "21"
                    }
                }
            };

            var deepDiff = CreateDeepDiff();
            var listener = new StoreAllOperationListener();
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
            var operations = listener.Operations;

            Assert.Null(result);
            Assert.Empty(operations);
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled)]
        [InlineData(EqualityComparers.Naive)]
        public void InsertEntity(EqualityComparers equalityComparer)
        {
            var existingEntity = (Entity)null!;

            var newEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntityBase>
                {
                    new SubEntity1
                    {
                        Key = 11,
                        Name1 = "11"
                    },
                    new SubEntity1
                    {
                        Key = 12,
                        Name1 = "12"
                    },
                    new SubEntity2
                    {
                        Key = 21,
                        Name2 = "21"
                    }
                }
            };

            var deepDiff = CreateDeepDiff();
            var listener = new StoreAllOperationListener();
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
            var operations = listener.Operations;

            Assert.NotNull(result);
            Assert.Equal(4, operations.Count);
            Assert.Equal(PersistChange.Insert, result.PersistChange);
            Assert.All(result.SubEntities, x => Assert.Equal(PersistChange.Insert, x.PersistChange));
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled)]
        [InlineData(EqualityComparers.Naive)]
        public void InsertSubEntity(EqualityComparers equalityComparer)
        {
            var existingEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntityBase>
                {
                    new SubEntity1
                    {
                        Key = 11,
                        Name1 = "11"
                    },
                    new SubEntity2
                    {
                        Key = 21,
                        Name2 = "21"
                    }
                }
            };

            var newEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntityBase>
                {
                    new SubEntity1
                    {
                        Key = 11,
                        Name1 = "11"
                    },
                    new SubEntity1 // <-- new one
                    {
                        Key = 12,
                        Name1 = "12"
                    },
                    new SubEntity2
                    {
                        Key = 21,
                        Name2 = "21"
                    }
                }
            };

            var deepDiff = CreateDeepDiff();
            var listener = new StoreAllOperationListener();
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
            var operations = listener.Operations;

            Assert.NotNull(result);
            Assert.Single(operations);
            Assert.Equal(PersistChange.None, result.PersistChange);
            Assert.Equal(PersistChange.Insert, result.SubEntities.Single().PersistChange);
            Assert.Equal(12, result.SubEntities.Single().Key);
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled)]
        [InlineData(EqualityComparers.Naive)]
        public void Deletentity(EqualityComparers equalityComparer)
        {
            var existingEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntityBase>
                {
                    new SubEntity1
                    {
                        Key = 11,
                        Name1 = "11"
                    },
                    new SubEntity2
                    {
                        Key = 21,
                        Name2 = "21"
                    }
                }
            };

            var newEntity = (Entity)null!;

            var deepDiff = CreateDeepDiff();
            var listener = new StoreAllOperationListener();
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
            var operations = listener.Operations;

            Assert.NotNull(result);
            Assert.Equal(3, operations.Count);
            Assert.Equal(PersistChange.Delete, result.PersistChange);
            Assert.All(result.SubEntities, x => Assert.Equal(PersistChange.Delete, x.PersistChange));
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled)]
        [InlineData(EqualityComparers.Naive)]
        public void DeleteSubEntity(EqualityComparers equalityComparer)
        {
            var existingEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntityBase>
                {
                    new SubEntity1
                    {
                        Key = 11,
                        Name1 = "11"
                    },
                    new SubEntity2
                    {
                        Key = 21,
                        Name2 = "21"
                    }
                }
            };

            var newEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntityBase>
                {
                    new SubEntity2
                    {
                        Key = 21,
                        Name2 = "21"
                    }
                }
            };

            var deepDiff = CreateDeepDiff();
            var listener = new StoreAllOperationListener();
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
            var operations = listener.Operations;

            Assert.NotNull(result);
            Assert.Single(operations);
            Assert.Equal(PersistChange.None, result.PersistChange);
            Assert.Equal(PersistChange.Delete, result.SubEntities.Single().PersistChange);
            Assert.Equal(11, result.SubEntities.Single().Key);
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled)]
        [InlineData(EqualityComparers.Naive)]
        public void UpdateSubEntity(EqualityComparers equalityComparer)
        {
            var existingEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntityBase>
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
                SubEntities = new List<SubEntityBase>
                {
                    new SubEntity1
                    {
                        Key = 1001,
                        Name1 = "new 1001"
                    }
                }
            };

            var deepDiff = CreateDeepDiff();
            var listener = new StoreAllOperationListener();
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
            var operations = listener.Operations;

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
            diffConfiguration.ConfigureEntity<Entity>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.Key)
                .HasValues(x => x.Name)
                .HasMany(x => x.SubEntities);
            diffConfiguration.ConfigureEntity<SubEntity1>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.Key)
                .HasValues(x => x.Name1);
            diffConfiguration.ConfigureEntity<SubEntity2>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.Key)
                .HasValues(x => x.Name2);

            return diffConfiguration.CreateDeepDiff();
        }
    }
}
