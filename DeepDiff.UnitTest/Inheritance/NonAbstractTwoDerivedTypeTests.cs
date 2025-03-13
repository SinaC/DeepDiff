using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Inheritance.Entities.NonAbstract;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Inheritance
{
    public class NonAbstractTwoDerivedTypeTests
    {
        [Theory]
        [InlineData(EqualityComparers.Precompiled, true)]
        [InlineData(EqualityComparers.Precompiled, false)]
        [InlineData(EqualityComparers.Naive, true)]
        [InlineData(EqualityComparers.Naive, false)]
        public void Identical(EqualityComparers equalityComparer, bool useParallelism)
        {
            var existingEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntity>
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
                SubEntities = new List<SubEntity>
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
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer).UseParallelism(useParallelism));
            var operations = listener.Operations;

            Assert.Null(result);
            Assert.Empty(operations);
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled, true)]
        [InlineData(EqualityComparers.Precompiled, false)]
        [InlineData(EqualityComparers.Naive, true)]
        [InlineData(EqualityComparers.Naive, false)]
        public void InsertEntity(EqualityComparers equalityComparer, bool useParallelism)
        {
            var existingEntity = (Entity)null!;

            var newEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntity>
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
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer).UseParallelism(useParallelism));
            var operations = listener.Operations;

            Assert.NotNull(result);
            Assert.Equal(4, operations.Count);
            Assert.Equal(PersistChange.Insert, result.PersistChange);
            Assert.All(result.SubEntities, x => Assert.Equal(PersistChange.Insert, x.PersistChange));
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled, true)]
        [InlineData(EqualityComparers.Precompiled, false)]
        [InlineData(EqualityComparers.Naive, true)]
        [InlineData(EqualityComparers.Naive, false)]
        public void InsertSubEntity(EqualityComparers equalityComparer, bool useParallelism)
        {
            var existingEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntity>
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
                SubEntities = new List<SubEntity>
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
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer).UseParallelism(useParallelism));
            var operations = listener.Operations;

            Assert.NotNull(result);
            Assert.Single(operations);
            Assert.Equal(PersistChange.None, result.PersistChange);
            Assert.Equal(PersistChange.Insert, result.SubEntities.Single().PersistChange);
            Assert.Equal(12, result.SubEntities.Single().Key);
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled, true)]
        [InlineData(EqualityComparers.Precompiled, false)]
        [InlineData(EqualityComparers.Naive, true)]
        [InlineData(EqualityComparers.Naive, false)]
        public void DeleteEntity(EqualityComparers equalityComparer, bool useParallelism)
        {
            var existingEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntity>
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
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer).UseParallelism(useParallelism));
            var operations = listener.Operations;

            Assert.NotNull(result);
            Assert.Equal(3, operations.Count);
            Assert.Equal(PersistChange.Delete, result.PersistChange);
            Assert.All(result.SubEntities, x => Assert.Equal(PersistChange.Delete, x.PersistChange));
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled, true)]
        [InlineData(EqualityComparers.Precompiled, false)]
        [InlineData(EqualityComparers.Naive, true)]
        [InlineData(EqualityComparers.Naive, false)]
        public void DeleteSubEntity(EqualityComparers equalityComparer, bool useParallelism)
        {
            var existingEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntity>
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
                SubEntities = new List<SubEntity>
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
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer).UseParallelism(useParallelism));
            var operations = listener.Operations;

            Assert.NotNull(result);
            Assert.Single(operations);
            Assert.Equal(PersistChange.None, result.PersistChange);
            Assert.Equal(PersistChange.Delete, result.SubEntities.Single().PersistChange);
            Assert.Equal(11, result.SubEntities.Single().Key);
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled, true)]
        [InlineData(EqualityComparers.Precompiled, false)]
        [InlineData(EqualityComparers.Naive, true)]
        [InlineData(EqualityComparers.Naive, false)]
        public void UpdateSubEntity(EqualityComparers equalityComparer, bool useParallelism)
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
            var listener = new StoreAllOperationListener();
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer).UseParallelism(useParallelism));
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
            diffConfiguration.Entity<SubEntity2>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.Key)
                .HasValues(x => x.Name2);

            return diffConfiguration.CreateDeepDiff();
        }
    }
}
