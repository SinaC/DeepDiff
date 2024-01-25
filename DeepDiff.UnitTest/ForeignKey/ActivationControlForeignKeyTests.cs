using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.ActivationControl;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.ForeignKey
{
    public class ActivationControlForeignKeyTests
    {
        [Fact]
        public void InsertDpTimestampDetail()
        {
            var deepDiff = CreateDeepDiff();

            //
            var (existingEntity, newEntity) = GenerateEntities();
            newEntity.ActivationControlDetails.First().DpDetails.First().TimestampDetails.Add(new ActivationControlDpTimestampDetail
            {
                Timestamp = newEntity.Day.LocalDateTime.AddSeconds(4),

                PowerMeasured = 100,
                PowerBaseline = 101,
                FcrCorrection = 102,
                EnergySupplied = 102
            });

            var diff = deepDiff.DiffSingle(existingEntity, newEntity);
            var result = diff.Entity;

            // only one modification at DpTimestampDetails
            Assert.NotNull(result);
            Assert.Equal(PersistChange.None, result.PersistChange);
            Assert.Single(result.ActivationControlDetails);
            Assert.Equal(PersistChange.None, result.ActivationControlDetails.Single().PersistChange);
            Assert.Single(result.ActivationControlDetails.Single().DpDetails);
            Assert.Equal(PersistChange.None, result.ActivationControlDetails.Single().DpDetails.Single().PersistChange);
            Assert.Single(result.ActivationControlDetails.Single().DpDetails.Single().TimestampDetails); // one insert
            Assert.Equal(PersistChange.Insert, result.ActivationControlDetails.Single().DpDetails.Single().TimestampDetails.Single().PersistChange);
            // check if FK are correct
            Assert.Equal(result.Id, result.ActivationControlDetails.Single().DpDetails.Single().TimestampDetails.Single().ActivationControlId);
            Assert.Equal(result.ActivationControlDetails.Single().StartsOn, result.ActivationControlDetails.Single().DpDetails.Single().TimestampDetails.Single().StartsOn);
            Assert.Equal(result.ActivationControlDetails.Single().DpDetails.Single().DeliveryPointEan, result.ActivationControlDetails.Single().DpDetails.Single().TimestampDetails.Single().DeliveryPointEan);
        }

        [Fact]
        public void InsertDpDetail()
        {
            var deepDiff = CreateDeepDiff();

            //
            var (existingEntity, newEntity) = GenerateEntities();
            newEntity.ActivationControlDetails.First().DpDetails.Add(new ActivationControlDpDetail
            {
                DeliveryPointEan = "DPEAN2",

                DeliveryPointName = "DP_NAME2",
                Direction = Direction.Up,
                DeliveryPointType = DeliveryPointType.ProvidingGroup,
                TotalEnergySupplied = 30,

                TimestampDetails = new List<ActivationControlDpTimestampDetail>
                {
                    new ActivationControlDpTimestampDetail
                    {
                        Timestamp = newEntity.Day.LocalDateTime,

                        PowerMeasured = 666,
                        PowerBaseline = 777,
                        FcrCorrection = 888,
                        EnergySupplied = 999
                    }
                }
            });

            var diff = deepDiff.DiffSingle(existingEntity, newEntity);
            var result = diff.Entity;

            // only one modification at DpTimestampDetails
            Assert.NotNull(result);
            Assert.Equal(PersistChange.None, result.PersistChange);
            Assert.Single(result.ActivationControlDetails);
            Assert.Equal(PersistChange.None, result.ActivationControlDetails.Single().PersistChange);
            Assert.Single(result.ActivationControlDetails.Single().DpDetails);
            Assert.Equal(PersistChange.Insert, result.ActivationControlDetails.Single().DpDetails.Single().PersistChange);
            // check if FK are correct
            Assert.Equal(result.Id, result.ActivationControlDetails.Single().DpDetails.Single().ActivationControlId);
            Assert.Equal(result.ActivationControlDetails.Single().StartsOn, result.ActivationControlDetails.Single().DpDetails.Single().StartsOn);
            Assert.Equal(result.ActivationControlDetails.Single().DpDetails.Single().DeliveryPointEan, result.ActivationControlDetails.Single().DpDetails.Single().DeliveryPointEan);
        }

        [Fact]
        public void NoExisting()
        {
            var deepDiff = CreateDeepDiff();

            //
            var (_, newEntity) = GenerateEntities();

            var diff = deepDiff.DiffSingle(null, newEntity);
            var result = diff.Entity;

            //
            Assert.NotNull(result);
            Assert.Equal(PersistChange.Insert, result.PersistChange);
            Assert.All(result.ActivationControlDetails, x => Assert.Equal(PersistChange.Insert, x.PersistChange));
            // check FK are not set
            Assert.All(result.ActivationControlDetails, x => Assert.All(x.TimestampDetails, y => Assert.NotEqual(x.StartsOn, y.StartsOn)));
            Assert.All(result.ActivationControlDetails, x => Assert.All(x.DpDetails, y => Assert.NotEqual(x.StartsOn, y.StartsOn)));
            Assert.All(result.ActivationControlDetails, x => Assert.All(x.DpDetails, y => Assert.All(y.TimestampDetails, z => Assert.NotEqual(x.StartsOn, z.StartsOn))));
            Assert.All(result.ActivationControlDetails, x => Assert.All(x.DpDetails, y => Assert.All(y.TimestampDetails, z => Assert.NotEqual(y.DeliveryPointEan, z.DeliveryPointEan))));
        }

        private (Entities.ActivationControl.ActivationControl existing, Entities.ActivationControl.ActivationControl calculated) GenerateEntities()
        {
            var existing = GenerateEntity(1);
            SetForeignKey(existing);

            var calculated = GenerateEntity(0);

            return (existing, calculated);
        }

        private void SetForeignKey(Entities.ActivationControl.ActivationControl activationControl)
        {
            // set FK
            foreach (var activationControlDetail in activationControl.ActivationControlDetails)
            {
                activationControlDetail.ActivationControlId = activationControl.Id;
                foreach (var activationControlTimestampDetail in activationControlDetail.TimestampDetails)
                {
                    activationControlTimestampDetail.ActivationControlId = activationControl.Id;
                    activationControlTimestampDetail.StartsOn = activationControlDetail.StartsOn;
                }
                foreach (var activationControlDpDetail in activationControlDetail.DpDetails)
                {
                    activationControlDpDetail.ActivationControlId = activationControl.Id;
                    activationControlDpDetail.StartsOn = activationControlDetail.StartsOn;
                    foreach (var activationControlDpTimestampDetail in activationControlDpDetail.TimestampDetails)
                    {
                        activationControlDpTimestampDetail.ActivationControlId = activationControl.Id;
                        activationControlDpTimestampDetail.StartsOn = activationControlDetail.StartsOn;
                        activationControlDpTimestampDetail.DeliveryPointEan = activationControlDpDetail.DeliveryPointEan;
                    }
                }
            }
        }

        private Entities.ActivationControl.ActivationControl GenerateEntity(int id)
        {
            var day = Date.Today;

            return new Entities.ActivationControl.ActivationControl
            {
                Id = id,

                ContractReference = "CREF",
                Day = day,

                TotalEnergyRequested = 1,
                TotalEnergyToBeSupplied = 2,
                TotalDiscrepancy = 3,
                FailedPercentage = 4,
                IsMeasurementExcludedCount = 5,
                IsJumpExcludedCount = 6,

                Status = ActivationControlStatus.Validated,
                InternalComment = "INTERNAL",
                TsoComment = "TSO",

                ActivationControlDetails = new List<ActivationControlDetail>
                {
                    new ActivationControlDetail
                    {
                        StartsOn = day.LocalDateTime,

                        OfferedVolumeUp = 7,
                        OfferedVolumeDown = 8,
                        OfferedVolumeForRedispatchingUp = 9,
                        OfferedVolumeForRedispatchingDown = 10,
                        PermittedDeviationUp = 11,
                        PermittedDeviationDown = 12,
                        RampingRate = 13,
                        HasJump = false,

                        TimestampDetails = new List<ActivationControlTimestampDetail>
                        {
                            new ActivationControlTimestampDetail
                            {
                                Timestamp = day.LocalDateTime,

                                PowerMeasured = 14,
                                PowerBaseline = 15,
                                FcrCorrection = 16,
                                EnergyRequested = 17,
                                EnergyRequestedForRedispatching = 18,
                                EnergySupplied = 19,
                                EnergyToBeSupplied = 20,
                                Deviation = 21,
                                PermittedDeviation = 22,
                                MaxDeviation = 23,
                                Discrepancy = 24,
                                IsJumpExcluded = false,
                                IsMeasurementExcluded = false,
                            }
                        },

                        DpDetails = new List<ActivationControlDpDetail>
                        {
                            new ActivationControlDpDetail
                            {
                                DeliveryPointEan = "DPEAN",

                                DeliveryPointName = "DP_NAME",
                                Direction = Direction.Up,
                                DeliveryPointType = DeliveryPointType.ProvidingGroup,
                                TotalEnergySupplied = 25,

                                TimestampDetails = new List<ActivationControlDpTimestampDetail>
                                {
                                    new ActivationControlDpTimestampDetail
                                    {
                                        Timestamp = day.LocalDateTime,

                                        PowerMeasured = 26,
                                        PowerBaseline = 27,
                                        FcrCorrection = 28,
                                        EnergySupplied = 29
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        private static IDeepDiff CreateDeepDiff()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.PersistEntity<Entities.ActivationControl.ActivationControl>()
                .HasKey(x => new { x.Day, x.ContractReference })
                .HasMany(x => x.ActivationControlDetails, cfg => cfg.HasNavigationKey(x => x.ActivationControlId, x => x.Id))
                .HasValues(x => new { x.TotalEnergyRequested, x.TotalDiscrepancy, x.TotalEnergyToBeSupplied, x.FailedPercentage, x.IsMeasurementExcludedCount, x.IsJumpExcludedCount })
                .OnUpdate(cfg => cfg.CopyValues(x => x.Status));
            diffConfiguration.PersistEntity<ActivationControlDetail>()
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.OfferedVolumeUp, x.OfferedVolumeDown, x.OfferedVolumeForRedispatchingUp, x.OfferedVolumeForRedispatchingDown, x.PermittedDeviationUp, x.PermittedDeviationDown, x.RampingRate, x.HasJump })
                .HasMany(x => x.TimestampDetails, cfg => cfg.HasNavigationKey(x => x.ActivationControlId, x => x.ActivationControlId).HasNavigationKey(x => x.StartsOn, x => x.StartsOn))
                .HasMany(x => x.DpDetails, cfg => cfg.HasNavigationKey(x => x.ActivationControlId, x => x.ActivationControlId).HasNavigationKey(x => x.StartsOn, x => x.StartsOn));
            diffConfiguration.PersistEntity<ActivationControlTimestampDetail>()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergyRequested, x.EnergyRequestedForRedispatching, x.EnergySupplied, x.EnergyToBeSupplied, x.Deviation, x.PermittedDeviation, x.MaxDeviation, x.Discrepancy, x.IsJumpExcluded, x.IsMeasurementExcluded });
            diffConfiguration.PersistEntity<ActivationControlDpDetail>()
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.DeliveryPointName, x.Direction, x.DeliveryPointType, x.TotalEnergySupplied })
                .HasMany(x => x.TimestampDetails, cfg => cfg.HasNavigationKey(x => x.ActivationControlId, x => x.ActivationControlId).HasNavigationKey(x => x.StartsOn, x => x.StartsOn).HasNavigationKey(x => x.DeliveryPointEan, x => x.DeliveryPointEan));
            diffConfiguration.PersistEntity<ActivationControlDpTimestampDetail>()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.PowerMeasured, x.PowerBaseline, x.FcrCorrection, x.EnergySupplied });
            var deepDiff = diffConfiguration.CreateDeepDiff();
            return deepDiff;
        }
    }
}
