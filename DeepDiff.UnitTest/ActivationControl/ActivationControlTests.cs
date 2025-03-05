using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.ActivationControl;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.ActivationControl
{
    public class ActivationControlTests
    {
        [Fact]
        public void ValidateIfEveryPropertiesAreReferenced_NoExtension()
        {
            var deepDiff = CreateDeepDiffWithoutExtensions();
            Assert.NotNull(deepDiff);
        }

        [Fact]
        public void ValidateIfEveryPropertiesAreReferenced_Extension()
        {
            var deepDiff = CreateDeepDiffWithExtensions();
            Assert.NotNull(deepDiff);
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled)]
        [InlineData(EqualityComparers.Naive)]
        public void Single_2Updates(EqualityComparers equalityComparer)
        {
            var deliveryDate = Date.Today;

            var existing = Generate(deliveryDate, ActivationControlStatus.Validated, "INTERNAL", "TSO");
            var calculated = Generate(deliveryDate, ActivationControlStatus.Calculated, null!, null!);
            calculated.TotalEnergyToBeSupplied = 5m;
            calculated.ActivationControlDetails[5].DpDetails[2].TimestampDetails[7].EnergySupplied = -7m;

            //
            var deepDiff = CreateDeepDiffWithoutExtensions();
            var listener = new StoreAllOperationListener();
            var result = deepDiff.MergeSingle(existing, calculated, listener, cfg => cfg.SetEqualityComparer(equalityComparer));

            //
            Assert.NotNull(result);
            Assert.Equal(PersistChange.Update, result.PersistChange);
            Assert.Equal(ActivationControlStatus.Calculated, result.Status);
            Assert.Equal("INTERNAL", result.InternalComment);
            Assert.Equal("TSO", result.TsoComment);
            Assert.Single(result.ActivationControlDetails);
            Assert.Single(result.ActivationControlDetails.Single().DpDetails);
            Assert.Empty(result.ActivationControlDetails.Single().TimestampDetails);
            Assert.Single(result.ActivationControlDetails.Single().DpDetails.Single().TimestampDetails);
            Assert.Equal(-7, result.ActivationControlDetails.Single().DpDetails.Single().TimestampDetails.Single().EnergySupplied);
            Assert.Empty(listener.Operations.OfType<InsertDiffOperation>());
            Assert.Empty(listener.Operations.OfType<DeleteDiffOperation>());
            Assert.Equal(2, listener.Operations.OfType<UpdateDiffOperation>().Count());
            Assert.Single(listener.Operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(Entities.ActivationControl.ActivationControl)));
            Assert.Single(listener.Operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(Entities.ActivationControl.ActivationControl)).SelectMany(x => x.UpdatedProperties));
            Assert.Equal((5m).ToString(), listener.Operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(Entities.ActivationControl.ActivationControl)).UpdatedProperties.Single().NewValue);
            Assert.Equal((3).ToString(), listener.Operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(Entities.ActivationControl.ActivationControl)).UpdatedProperties.Single().ExistingValue);
            Assert.Equal(nameof(Entities.ActivationControl.ActivationControl.TotalEnergyToBeSupplied), listener.Operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControl)).UpdatedProperties.Single().PropertyName);
            Assert.Equal(2, listener.Operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControl)).Keys.Count);
            Assert.Single(listener.Operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControl)).Keys.Where(x => x.Key == nameof(Entities.ActivationControl.ActivationControl.Day)));
            Assert.Single(listener.Operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControl)).Keys.Where(x => x.Key == nameof(Entities.ActivationControl.ActivationControl.ContractReference)));
            Assert.Equal($"{deliveryDate}", listener.Operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControl)).Keys.Single(x => x.Key == nameof(Entities.ActivationControl.ActivationControl.Day)).Value);
            Assert.Equal("CREF", listener.Operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControl)).Keys.Single(x => x.Key == nameof(Entities.ActivationControl.ActivationControl.ContractReference)).Value);
            Assert.Single(listener.Operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(ActivationControlDpTimestampDetail)));
            Assert.Equal((-7m).ToString(), listener.Operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControlDpTimestampDetail)).UpdatedProperties.Single().NewValue);
            Assert.Equal((420).ToString(), listener.Operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControlDpTimestampDetail)).UpdatedProperties.Single().ExistingValue);
            Assert.Equal(nameof(ActivationControlDpTimestampDetail.EnergySupplied), listener.Operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControlDpTimestampDetail)).UpdatedProperties.Single().PropertyName);
            Assert.Single(listener.Operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControlDpTimestampDetail)).Keys);
            Assert.Equal(nameof(ActivationControlDpTimestampDetail.Timestamp), listener.Operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControlDpTimestampDetail)).Keys.Single().Key);
            Assert.Equal($"{deliveryDate.UtcDateTime.AddMinutes(5*15).AddSeconds(7*4)}", listener.Operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControlDpTimestampDetail)).Keys.Single().Value);
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled)]
        [InlineData(EqualityComparers.Naive)]
        public void Decimal6(EqualityComparers equalityComparer)
        {
            var deliveryDate = Date.Today;
            var existing = new Entities.ActivationControl.ActivationControl
            {
                Day = deliveryDate,
                ContractReference = "CREF",

                TotalEnergyToBeSupplied = 1.123456789m,
            };
            var calculated = new Entities.ActivationControl.ActivationControl
            {
                Day = deliveryDate,
                ContractReference = "CREF",

                TotalEnergyToBeSupplied = 1.1234561212121m,
            };

            //
            var deepDiff = CreateDeepDiffWithoutExtensions();
            var listener = new StoreAllOperationListener();
            var result = deepDiff.MergeSingle(existing, calculated, listener, cfg => cfg.SetEqualityComparer(equalityComparer));

            //
            Assert.Null(result); // no diff
            Assert.Empty(listener.Operations);
        }

        private static IDeepDiff CreateDeepDiffWithoutExtensions()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<Entities.ActivationControl.ActivationControl>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .WithComparer(new DecimalComparer(6))
                .WithComparer(new NullableDecimalComparer(6))
                .HasKey(x => new { x.Day, x.ContractReference })
                .HasMany(x => x.ActivationControlDetails)
                .HasValues(x => new { x.TotalEnergyRequested, x.TotalDiscrepancy, x.TotalEnergyToBeSupplied, x.FailedPercentage, x.IsMeasurementExcludedCount, x.IsJumpExcludedCount })
                .OnUpdate(cfg => cfg.CopyValues(x => x.Status))
                .Ignore(x => new { x.PersistChange, x.CreatedOn, x.CreatedBy, x.UpdatedOn, x.UpdatedBy, x.Id, x.InternalComment, x.TsoComment });
            diffConfiguration.Entity<ActivationControlDetail>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .WithComparer(new DecimalComparer(6))
                .WithComparer(new NullableDecimalComparer(6))
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.OfferedVolumeUp, x.OfferedVolumeDown, x.OfferedVolumeForRedispatchingUp, x.OfferedVolumeForRedispatchingDown, x.PermittedDeviationUp, x.PermittedDeviationDown, x.RampingRate, x.HasJump })
                .HasMany(x => x.TimestampDetails)
                .HasMany(x => x.DpDetails)
                .Ignore(x => new { x.PersistChange, x.ActivationControlId, x.ActivationControl });
            diffConfiguration.Entity<ActivationControlTimestampDetail>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .WithComparer(new DecimalComparer(6))
                .WithComparer(new NullableDecimalComparer(6))
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergyRequested, x.EnergyRequestedForRedispatching, x.EnergySupplied, x.EnergyToBeSupplied, x.Deviation, x.PermittedDeviation, x.MaxDeviation, x.Discrepancy, x.IsJumpExcluded, x.IsMeasurementExcluded })
                .Ignore(x => new { x.PersistChange, x.ActivationControlId, x.StartsOn, x.ActivationControlDetail, x.AuditedOn, x.AuditedBy });
            diffConfiguration.Entity<ActivationControlDpDetail>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .WithComparer(new DecimalComparer(6))
                .WithComparer(new NullableDecimalComparer(6))
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.DeliveryPointName, x.Direction, x.DeliveryPointType, x.TotalEnergySupplied })
                .HasMany(x => x.TimestampDetails)
                .Ignore(x => new { x.PersistChange, x.ActivationControlId, x.StartsOn, x.ActivationControlDetail });
            diffConfiguration.Entity<ActivationControlDpTimestampDetail>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .WithComparer(new DecimalComparer(6))
                .WithComparer(new NullableDecimalComparer(6))
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergySupplied })
                .Ignore(x => new { x.PersistChange, x.ActivationControlId, x.StartsOn, x.DeliveryPointEan, x.ActivationControlDpDetail });

            diffConfiguration.ValidateIfEveryPropertiesAreReferenced();

            var deepDiff = diffConfiguration.CreateDeepDiff();
            return deepDiff;
        }

        private static IDeepDiff CreateDeepDiffWithExtensions()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.PersistEntity<Entities.ActivationControl.ActivationControl>()
                .HasKey(x => new { x.Day, x.ContractReference })
                .HasMany(x => x.ActivationControlDetails)
                .HasValues(x => new { x.TotalEnergyRequested, x.TotalDiscrepancy, x.TotalEnergyToBeSupplied, x.FailedPercentage, x.IsMeasurementExcludedCount, x.IsJumpExcludedCount })
                .OnUpdate(cfg => cfg.CopyValues(x => x.Status))
                .IgnoreUpdateAudit()
                .Ignore(x => new { x.InternalComment, x.TsoComment });
            diffConfiguration.PersistEntity<ActivationControlDetail>()
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.OfferedVolumeUp, x.OfferedVolumeDown, x.OfferedVolumeForRedispatchingUp, x.OfferedVolumeForRedispatchingDown, x.PermittedDeviationUp, x.PermittedDeviationDown, x.RampingRate, x.HasJump })
                .HasMany(x => x.TimestampDetails)
                .HasMany(x => x.DpDetails)
                .Ignore(x => new { x.ActivationControlId, x.ActivationControl });
            diffConfiguration.PersistEntity<ActivationControlTimestampDetail>()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergyRequested, x.EnergyRequestedForRedispatching, x.EnergySupplied, x.EnergyToBeSupplied, x.Deviation, x.PermittedDeviation, x.MaxDeviation, x.Discrepancy, x.IsJumpExcluded, x.IsMeasurementExcluded })
                .IgnoreAudit()
                .Ignore(x => new { x.ActivationControlId, x.StartsOn, x.ActivationControlDetail, x.AuditedOn, x.AuditedBy });
            diffConfiguration.PersistEntity<ActivationControlDpDetail>()
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.DeliveryPointName, x.Direction, x.DeliveryPointType, x.TotalEnergySupplied })
                .HasMany(x => x.TimestampDetails)
                .Ignore(x => new { x.ActivationControlId, x.StartsOn, x.ActivationControlDetail });
            diffConfiguration.PersistEntity<ActivationControlDpTimestampDetail>()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergySupplied })
                .Ignore(x => new { x.ActivationControlId, x.StartsOn, x.DeliveryPointEan, x.ActivationControlDpDetail });

            diffConfiguration.ValidateIfEveryPropertiesAreReferenced();

            var deepDiff = diffConfiguration.CreateDeepDiff();
            return deepDiff;
        }

        private static Entities.ActivationControl.ActivationControl Generate(Date deliveryDate, ActivationControlStatus status, string internalComment, string tsoComment)
            => new()
            {
                Id = 1,

                Day = deliveryDate,
                ContractReference = "CREF",

                TotalEnergyRequested = 1,
                TotalDiscrepancy = 2,
                TotalEnergyToBeSupplied = 3,

                Status = status,
                InternalComment = internalComment,
                TsoComment = tsoComment,

                ActivationControlDetails = Enumerable.Range(0, 96)
                    .Select(x => new ActivationControlDetail
                    {
                        ActivationControlId = 1,
                        StartsOn = deliveryDate.UtcDateTime.AddMinutes(15 * x),

                        OfferedVolumeUp = x,
                        OfferedVolumeDown = 2 * x,
                        OfferedVolumeForRedispatchingUp = 3 * x,
                        OfferedVolumeForRedispatchingDown = 4 * x,
                        PermittedDeviationUp = 5 * x,
                        PermittedDeviationDown = 6 * x,
                        RampingRate = 7 * x,
                        HasJump = x % 2 == 0,

                        TimestampDetails = Enumerable.Range(0, 255)
                            .Select(y => new ActivationControlTimestampDetail
                            {
                                ActivationControlId = 1,
                                Timestamp = deliveryDate.UtcDateTime.AddMinutes(15 * x).AddSeconds(4 * y),

                                PowerMeasured = x * y,
                                PowerBaseline = 2 * x * y,
                                FcrCorrection = 3 * x * y,
                                EnergyRequested = 4 * x * y,
                                EnergyRequestedForRedispatching = 5 * x * y,
                                EnergySupplied = 6 * x * y,
                                EnergyToBeSupplied = 7 * x * y,
                                Deviation = 8 * x * y,
                                PermittedDeviation = 9 * x * y,
                                MaxDeviation = 10 * x * y,
                                Discrepancy = 11 * x * y,
                                IsJumpExcluded = (x + y) % 2 == 0,
                                IsMeasurementExcluded = false,
                            }).ToList(),

                        DpDetails = Enumerable.Range(0, 5)
                            .Select(y => new ActivationControlDpDetail
                            {
                                ActivationControlId = 1,
                                DeliveryPointEan = $"DPEAN_{(x+1) * (y+1)}",

                                DeliveryPointName = $"DPNAME_{(x+1) * (y+1)}",
                                Direction = (x * y) % 2 == 0 ? Direction.Up : Direction.Down,
                                DeliveryPointType = (2 * x * y) % 2 == 0 ? DeliveryPointType.SingleUnit : DeliveryPointType.ProvidingGroup,
                                TotalEnergySupplied = 3 * x * y,

                                TimestampDetails = Enumerable.Range(0, 255)
                                    .Select(z => new ActivationControlDpTimestampDetail
                                    {
                                        ActivationControlId = 1,
                                        DeliveryPointEan = $"DPEAN_{x * y}",
                                        Timestamp = deliveryDate.UtcDateTime.AddMinutes(15 * x).AddSeconds(4 * z),

                                        PowerMeasured = x * y * z,
                                        PowerBaseline = 2 * x * y * z,
                                        FcrCorrection = 3 * x * y * z,
                                        EnergySupplied = 6 * x * y * z,
                                    }).ToList(),
                            }).ToList(),

                    }).ToList()
            };
    }
}
