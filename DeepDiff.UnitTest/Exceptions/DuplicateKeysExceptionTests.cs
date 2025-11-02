using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.UnitTest.Entities.Simple;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class DuplicateKeysExceptionTests
    {
        [Fact]
        public void SimpleKey_Insert_Hashtable()
        {
            var existingEntities = Array.Empty<EntityLevel2>();

            var newEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel2
            {
                DeliveryPointEan = $"DP_{x % 3}",
                Value1 = 2 * x,
                Value2 = 2 * x + 1,
            }).ToArray();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<EntityLevel2>()
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.Value1, x.Value2 });
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var ex = Assert.Throws<DuplicateKeysException>(() => deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.HashtableThreshold(1)));
            Assert.Equal(typeof(EntityLevel2), ex.EntityType);
            Assert.Equal("DP_0", ex.Keys);
        }

        [Fact]
        public void SimpleKey_Insert_BypassHashtable()
        {
            var existingEntities = Array.Empty<EntityLevel2>();

            var newEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel2
            {
                DeliveryPointEan = $"DP_{x % 3}",
                Value1 = 2 * x,
                Value2 = 2 * x + 1,
            }).ToArray();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<EntityLevel2>()
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.Value1, x.Value2 });
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var ex = Assert.Throws<DuplicateKeysException>(() => deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.HashtableThreshold(1000000))); // simulate no hashtable
            Assert.Equal(typeof(EntityLevel2), ex.EntityType);
            Assert.Equal("DP_0", ex.Keys);
        }

        [Fact]
        public void ComplexKey_Insert_Hashtable()
        {
            var existingEntities = Array.Empty<EntityLevel0>();

            var newEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel0
            {
                StartsOn = DateTime.Today.AddHours(x % 3),
                Direction = Direction.Up,

                RequestedPower = 11m
            }).ToArray();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.RequestedPower);
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var ex = Assert.Throws<DuplicateKeysException>(() => deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.HashtableThreshold(1)));
            Assert.Equal(typeof(EntityLevel0), ex.EntityType);
            Assert.Equal($"{DateTime.Today},Up", ex.Keys);
        }

        [Fact]
        public void ComplexKey_Insert_BypassHashtable()
        {
            var existingEntities = Array.Empty<EntityLevel0>();

            var newEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel0
            {
                StartsOn = DateTime.Today.AddHours(x % 3),
                Direction = Direction.Up,

                RequestedPower = 11m
            }).ToArray();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.RequestedPower);
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var ex = Assert.Throws<DuplicateKeysException>(() => deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.HashtableThreshold(1000000))); // simulate no hashtable
            Assert.Equal(typeof(EntityLevel0), ex.EntityType);
            Assert.Equal($"{DateTime.Today},Up", ex.Keys);
        }

        [Fact]
        public void SimpleKey_Delete_Hashtable()
        {
            var existingEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel2
            {
                DeliveryPointEan = $"DP_{x % 3}",
                Value1 = 2 * x,
                Value2 = 2 * x + 1,
            }).ToArray();

            var newEntities = Array.Empty<EntityLevel2>();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<EntityLevel2>()
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.Value1, x.Value2 });
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var ex = Assert.Throws<DuplicateKeysException>(() => deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.HashtableThreshold(1)));
            Assert.Equal(typeof(EntityLevel2), ex.EntityType);
            Assert.Equal("DP_0", ex.Keys);
        }

        [Fact]
        public void SimpleKey_Delete_BypassHashtable()
        {
            var existingEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel2
            {
                DeliveryPointEan = $"DP_{x % 3}",
                Value1 = 2 * x,
                Value2 = 2 * x + 1,
            }).ToArray();

            var newEntities = Array.Empty<EntityLevel2>();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<EntityLevel2>()
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.Value1, x.Value2 });
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var ex = Assert.Throws<DuplicateKeysException>(() => deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.HashtableThreshold(1000000))); // simulate no hashtable
            Assert.Equal(typeof(EntityLevel2), ex.EntityType);
            Assert.Equal("DP_0", ex.Keys);
        }

        [Fact]
        public void ComplexKey_Delete_Hashtable()
        {
            var existingEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel0
            {
                StartsOn = DateTime.Today.AddHours(x % 3),
                Direction = Direction.Up,

                RequestedPower = 11m
            }).ToArray();

            var newEntities = Array.Empty<EntityLevel0>();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.RequestedPower);
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var ex = Assert.Throws<DuplicateKeysException>(() => deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.HashtableThreshold(1)));
            Assert.Equal(typeof(EntityLevel0), ex.EntityType);
            Assert.Equal($"{DateTime.Today},Up", ex.Keys);
        }

        [Fact]
        public void ComplexKey_Delete_BypassHashtable()
        {
            var existingEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel0
            {
                StartsOn = DateTime.Today.AddHours(x % 3),
                Direction = Direction.Up,

                RequestedPower = 11m
            }).ToArray();

            var newEntities = Array.Empty<EntityLevel0>();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.RequestedPower);
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var ex = Assert.Throws<DuplicateKeysException>(() => deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.HashtableThreshold(1000000))); // simulate no hashtable
            Assert.Equal(typeof(EntityLevel0), ex.EntityType);
            Assert.Equal($"{DateTime.Today},Up", ex.Keys);
        }

        [Fact]
        public void SimpleKey_Update_Hashtable()
        {
            var existingEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel2
            {
                DeliveryPointEan = $"DP_{x % 3}",
                Value1 = x,
                Value2 = x + 1,
            }).ToArray();

            var newEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel2
            {
                DeliveryPointEan = $"DP_{x % 3}",
                Value1 = 2 * x,
                Value2 = 2 * x + 1,
            }).ToArray();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<EntityLevel2>()
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.Value1, x.Value2 });
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var ex = Assert.Throws<DuplicateKeysException>(() => deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.HashtableThreshold(1)));
            Assert.Equal(typeof(EntityLevel2), ex.EntityType);
            Assert.Equal("DP_0", ex.Keys);
        }

        [Fact]
        public void SimpleKey_Update_BypassHashtable()
        {
            var existingEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel2
            {
                DeliveryPointEan = $"DP_{x % 3}",
                Value1 = x,
                Value2 = x + 1,
            }).ToArray();

            var newEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel2
            {
                DeliveryPointEan = $"DP_{x % 3}",
                Value1 = 2 * x,
                Value2 = 2 * x + 1,
            }).ToArray();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<EntityLevel2>()
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.Value1, x.Value2 });
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var ex = Assert.Throws<DuplicateKeysException>(() => deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.HashtableThreshold(1000000))); // simulate no hashtable
            Assert.Equal(typeof(EntityLevel2), ex.EntityType);
            Assert.Equal("DP_0", ex.Keys);
        }

        [Fact]
        public void ComplexKey_Update_Hashtable()
        {
            var existingEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel0
            {
                StartsOn = DateTime.Today.AddHours(x % 3),
                Direction = Direction.Up,

                RequestedPower = 10m
            }).ToArray();

            var newEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel0
            {
                StartsOn = DateTime.Today.AddHours(x % 3),
                Direction = Direction.Up,

                RequestedPower = 11m
            }).ToArray();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.RequestedPower);
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var ex = Assert.Throws<DuplicateKeysException>(() => deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.HashtableThreshold(1)));
            Assert.Equal(typeof(EntityLevel0), ex.EntityType);
            Assert.Equal($"{DateTime.Today},Up", ex.Keys);
        }

        [Fact]
        public void ComplexKey_Update_BypassHashtable()
        {
            var existingEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel0
            {
                StartsOn = DateTime.Today.AddHours(x % 3),
                Direction = Direction.Up,

                RequestedPower = 10m
            }).ToArray();

            var newEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel0
            {
                StartsOn = DateTime.Today.AddHours(x % 3),
                Direction = Direction.Up,

                RequestedPower = 11m
            }).ToArray();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.RequestedPower);
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var ex = Assert.Throws<DuplicateKeysException>(() => deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.HashtableThreshold(1000000))); // simulate no hashtable
            Assert.Equal(typeof(EntityLevel0), ex.EntityType);
            Assert.Equal($"{DateTime.Today},Up", ex.Keys);
        }
    }
}
