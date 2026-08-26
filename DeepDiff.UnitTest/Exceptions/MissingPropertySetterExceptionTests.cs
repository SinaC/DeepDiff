using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Invalid;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions;

public class MissingPropertySetterExceptionTests
{
    [Fact]
    public void Should_ThrowMissingPropertySetterException_When_Property_Lacks_Setter()
    {
        var existingEntities = Enumerable.Range(0, 10).Select(x => new EntityNoSetter0
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"Existing{x}",
            AdditionalValueToCopy = $"ExistingAdditionalValue{x}",
        }).ToArray();
        foreach (var entity in existingEntities)
        {
            entity.SubEntities.AddRange(Enumerable.Range(0, 5).Select(y => new EntityNoSetter1
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddHours(entity.Index).AddMinutes(y),
                Price = y % 2 == 0 ? null : y * 3,
                Comment = $"Existing{entity.Index}_{y}",
            }).ToList());
        }

        var newEntities = Enumerable.Range(1, 10).Select(x => new EntityNoSetter0
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 3,
            Comment = $"New{x}",
            AdditionalValueToCopy = $"NewAdditionalValue{x}",
        }).ToArray();
        foreach (var entity in newEntities)
        {
            entity.SubEntities.AddRange(Enumerable.Range(1, 5).Select(y => new EntityNoSetter1
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddHours(entity.Index).AddMinutes(y),
                Price = y % 2 == 0 ? null : y * 3,
                Comment = $"New{entity.Index}_{y}",
            }).ToList());
        }

        var diffConfiguration = new DeepDiffConfiguration();
        var ex0 = Assert.Throws<MissingPropertySetterException>(() =>
        {
            diffConfiguration.ConfigureEntity<EntityNoSetter0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .OnUpdate(cfg => cfg.CopyValues(x => x.AdditionalValueToCopy))
                .HasMany(x => x.SubEntities);
        });
        Assert.Equal($"Property SubEntities of type {typeof(EntityNoSetter0).FullName} does not have a setter and cannot be used in configuration", ex0.Message);
        Assert.Equal(typeof(EntityNoSetter0), ex0.EntityType);

        var ex1 = Assert.Throws<MissingPropertySetterException>(() =>
        {
            diffConfiguration.ConfigureEntity<EntityNoSetter1>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.Power, x.Price });
        });
        Assert.Equal($"Property Power of type {typeof(EntityNoSetter1).FullName} does not have a setter and cannot be used in configuration", ex1.Message);
        Assert.Equal(typeof(EntityNoSetter1), ex1.EntityType);

        diffConfiguration.ValidateConfiguration();

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var results = deepDiff.MergeMany(existingEntities, newEntities).ToArray();
    }
}
