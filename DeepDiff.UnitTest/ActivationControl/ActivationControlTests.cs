using DeepDiff.Configuration;
using DeepDiff.Operations;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.ActivationControl;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.ActivationControl
{
    public class ActivationControlTests
    {
        [Fact]
        public void Single_2Updates()
        {
            var deliveryDate = Date.Today;

            var existing = Generate(deliveryDate, ActivationControlStatus.Validated, "INTERNAL", "TSO");
            var calculated = Generate(deliveryDate, ActivationControlStatus.Calculated, null!, null!);
            calculated.TotalEnergyToBeSupplied = 5m;
            calculated.ActivationControlDetails[5].DpDetails[2].TimestampDetails[7].EnergySupplied = -7m;

            //
            var deepDiff = CreateDeepDiff();
            var diff = deepDiff.DiffSingle(existing, calculated);
            var result = diff.Entity;
            var operations = diff.Operations;

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
            Assert.Empty(operations.OfType<InsertDiffOperation>());
            Assert.Empty(operations.OfType<DeleteDiffOperation>());
            Assert.Equal(2, operations.OfType<UpdateDiffOperation>().Count());
            Assert.Single(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(Entities.ActivationControl.ActivationControl)));
            Assert.Single(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(Entities.ActivationControl.ActivationControl)).SelectMany(x => x.UpdatedProperties));
            Assert.Equal((5m).ToString(), operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(Entities.ActivationControl.ActivationControl)).UpdatedProperties.Single().NewValue);
            Assert.Equal((3).ToString(), operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(Entities.ActivationControl.ActivationControl)).UpdatedProperties.Single().ExistingValue);
            Assert.Equal(nameof(Entities.ActivationControl.ActivationControl.TotalEnergyToBeSupplied), operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControl)).UpdatedProperties.Single().PropertyName);
            Assert.Single(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(ActivationControlDpTimestampDetail)));
            Assert.Equal((-7m).ToString(), operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControlDpTimestampDetail)).UpdatedProperties.Single().NewValue);
            Assert.Equal((420).ToString(), operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControlDpTimestampDetail)).UpdatedProperties.Single().ExistingValue);
            Assert.Equal(nameof(ActivationControlDpTimestampDetail.EnergySupplied), operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControlDpTimestampDetail)).UpdatedProperties.Single().PropertyName);
        }

        [Fact]
        public void Single_2Updates_Naive()
        {
            var deliveryDate = Date.Today;

            var existing = Generate(deliveryDate, ActivationControlStatus.Validated, "INTERNAL", "TSO");
            var calculated = Generate(deliveryDate, ActivationControlStatus.Calculated, null!, null!);
            calculated.TotalEnergyToBeSupplied = 5m;
            calculated.ActivationControlDetails[5].DpDetails[2].TimestampDetails[7].EnergySupplied = -7m;

            //
            var deepDiff = CreateDeepDiff();
            var diff = deepDiff.DiffSingle(existing, calculated, cfg => cfg.DisablePrecompiledEqualityComparer());
            var result = diff.Entity;
            var operations = diff.Operations;

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
            Assert.Empty(operations.OfType<InsertDiffOperation>());
            Assert.Empty(operations.OfType<DeleteDiffOperation>());
            Assert.Equal(2, operations.OfType<UpdateDiffOperation>().Count());
            Assert.Single(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(Entities.ActivationControl.ActivationControl)));
            Assert.Single(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(Entities.ActivationControl.ActivationControl)).SelectMany(x => x.UpdatedProperties));
            Assert.Equal((5m).ToString(), operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(Entities.ActivationControl.ActivationControl)).UpdatedProperties.Single().NewValue);
            Assert.Equal((3).ToString(), operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(Entities.ActivationControl.ActivationControl)).UpdatedProperties.Single().ExistingValue);
            Assert.Equal(nameof(Entities.ActivationControl.ActivationControl.TotalEnergyToBeSupplied), operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControl)).UpdatedProperties.Single().PropertyName);
            Assert.Single(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(ActivationControlDpTimestampDetail)));
            Assert.Equal((-7m).ToString(), operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControlDpTimestampDetail)).UpdatedProperties.Single().NewValue);
            Assert.Equal((420).ToString(), operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControlDpTimestampDetail)).UpdatedProperties.Single().ExistingValue);
            Assert.Equal(nameof(ActivationControlDpTimestampDetail.EnergySupplied), operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(ActivationControlDpTimestampDetail)).UpdatedProperties.Single().PropertyName);
        }

        [Fact]
        public void Decimal6()
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
            var deepDiff = CreateDeepDiff();
            var diff = deepDiff.DiffSingle(existing, calculated);
            var result = diff.Entity;
            var operations = diff.Operations;

            //
            Assert.Null(result); // no diff
        }

        [Fact]
        public void Decimal6_Naive()
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
            var deepDiff = CreateDeepDiff();
            var diff = deepDiff.DiffSingle(existing, calculated, cfg => cfg.DisablePrecompiledEqualityComparer());
            var result = diff.Entity;
            var operations = diff.Operations;

            //
            Assert.Null(result); // no diff
        }

        private static IDeepDiff CreateDeepDiff()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.ActivationControl.ActivationControl>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .WithComparer(new DecimalComparer(6))
                .WithComparer(new NullableDecimalComparer(6))
                .HasKey(x => new { x.Day, x.ContractReference })
                .HasMany(x => x.ActivationControlDetails)
                .HasValues(x => new { x.TotalEnergyRequested, x.TotalDiscrepancy, x.TotalEnergyToBeSupplied, x.FailedPercentage, x.IsMeasurementExcludedCount, x.IsJumpExcludedCount })
                .OnUpdate(cfg => cfg.CopyValues(x => x.Status));
            diffConfiguration.Entity<ActivationControlDetail>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .WithComparer(new DecimalComparer(6))
                .WithComparer(new NullableDecimalComparer(6))
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.OfferedVolumeUp, x.OfferedVolumeDown, x.OfferedVolumeForRedispatchingUp, x.OfferedVolumeForRedispatchingDown, x.PermittedDeviationUp, x.PermittedDeviationDown, x.RampingRate, x.HasJump })
                .HasMany(x => x.TimestampDetails)
                .HasMany(x => x.DpDetails);
            diffConfiguration.Entity<ActivationControlTimestampDetail>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .WithComparer(new DecimalComparer(6))
                .WithComparer(new NullableDecimalComparer(6))
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergyRequested, x.EnergyRequestedForRedispatching, x.EnergySupplied, x.EnergyToBeSupplied, x.Deviation, x.PermittedDeviation, x.MaxDeviation, x.Discrepancy, x.IsJumpExcluded, x.IsMeasurementExcluded });
            diffConfiguration.Entity<ActivationControlDpDetail>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .WithComparer(new DecimalComparer(6))
                .WithComparer(new NullableDecimalComparer(6))
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.DeliveryPointName, x.Direction, x.DeliveryPointType, x.TotalEnergySupplied })
                .HasMany(x => x.TimestampDetails);
            diffConfiguration.Entity<ActivationControlDpTimestampDetail>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .WithComparer(new DecimalComparer(6))
                .WithComparer(new NullableDecimalComparer(6))
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergySupplied });

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
                                DeliveryPointEan = $"DPEAN_{x * y}",

                                DeliveryPointName = $"DPNAME_{x * y}",
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
