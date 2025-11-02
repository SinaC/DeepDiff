using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Inheritance.Entities.Abstract;
using Xunit;

namespace DeepDiff.UnitTest.Inheritance
{
    public class AbstractWithOneDerivedTypeTests
    {
        [Fact]
        public void Identical()
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
                        Name1 = "1001"
                    }
                }
            };

            var deepDiff = CreateDeepDiff();
            var listener = new StoreAllOperationListener();
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener);
            var operations = listener.Operations;

            Assert.Null(result);
            Assert.Empty(operations);
        }

        [Fact]
        public void InsertEntity()
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
                 Key = 1002,
                 Name1 = "1002"
             }
         }
            };

            var deepDiff = CreateDeepDiff();
            var listener = new StoreAllOperationListener();
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener);
            var operations = listener.Operations;

            Assert.NotNull(result);
            Assert.Equal(2, operations.Count);
            Assert.Equal(PersistChange.Insert, result.PersistChange);
            Assert.Equal(PersistChange.Insert, result.SubEntities.Single().PersistChange);
            Assert.Equal(1002, result.SubEntities.Single().Key);
        }

        [Fact]
        public void InsertSubEntity()
        {
            var existingEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntityBase>()
            };

            var newEntity = new Entity
            {
                Key = 1,
                Name = "1",
                SubEntities = new List<SubEntityBase>
                {
                    new SubEntity1
                    {
                        Key = 1002,
                        Name1 = "1002"
                    }
                }
            };

            var deepDiff = CreateDeepDiff();
            var listener = new StoreAllOperationListener();
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener);
            var operations = listener.Operations;

            Assert.NotNull(result);
            Assert.Single(operations);
            Assert.Equal(PersistChange.None, result.PersistChange);
            Assert.Equal(PersistChange.Insert, result.SubEntities.Single().PersistChange);
            Assert.Equal(1002, result.SubEntities.Single().Key);
        }

        [Fact]
        public void DeleteEntity()
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
                    },
                    new SubEntity1
                    {
                        Key = 1002,
                        Name1 = "1002"
                    }
                }
            };

            var newEntity = (Entity)null!;

            var deepDiff = CreateDeepDiff();
            var listener = new StoreAllOperationListener();
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener);
            var operations = listener.Operations;

            Assert.NotNull(result);
            Assert.Equal(3, operations.Count);
            Assert.Equal(PersistChange.Delete, result.PersistChange);
            Assert.All(result.SubEntities, x => Assert.Equal(PersistChange.Delete, x.PersistChange));
        }

        [Fact]
        public void DeleteSubEntity()
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
                SubEntities = new List<SubEntityBase>
                {
                    new SubEntity1
                    {
                        Key = 1002,
                        Name1 = "1002"
                    }
                }
            };

            var deepDiff = CreateDeepDiff();
            var listener = new StoreAllOperationListener();
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener);
            var operations = listener.Operations;

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
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener);
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

            return diffConfiguration.CreateDeepDiff();
        }
    }
}
