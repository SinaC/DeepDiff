using DeepDiff;
using DeepDiff.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using TestAppNet5.Entities;
using TestAppNet5.Entities.ActivationControl;
using TestAppNet5.Entities.Simple;
using TestAppNet5.Profile;

namespace TestAppNet5
{
    public class Calculate : ICalculate
    {
        private ILogger Logger { get; }
        private IDeepDiff DeepDiff { get; }

        public Calculate(ILogger logger, IDeepDiff deepDiff)
        {
            Logger = logger;
            DeepDiff = deepDiff;
        }

        public void Perform(Date deliveryDate)
        {
            var entities = GenerateEntities(500);
            var deepDiff = CreateDeepDiff();
            var results = deepDiff.MergeMany(entities.existingEntities, entities.newEntities).ToArray();
        }

        public void Perform2(Date deliveryDate)
        {
            Logger.Information($"Start calculation for {deliveryDate}");

            var existing = Generate(deliveryDate, ActivationControlStatus.Validated, "INTERNAL", "TSO");
            var calculated = Generate(deliveryDate, ActivationControlStatus.Calculated, null, null);
            calculated.TotalEnergyToBeSupplied = 5m;
            calculated.ActivationControlDetails[5].DpDetails[2].TimestampDetails[7].EnergySupplied = -7m;

            var result = DeepDiff.MergeSingle(existing, calculated);

            Logger.Information($"result?: {result != null}");
        }

        private static ActivationControl Generate(Date deliveryDate, ActivationControlStatus status, string internalComment, string tsoComment)
            => new()
            {
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
                                DeliveryPointEan = $"DPEAN_{x * y}",

                                DeliveryPointName = $"DPNAME_{x * y}",
                                Direction = (x * y) % 2 == 0 ? Entities.ActivationControl.Direction.Up : Entities.ActivationControl.Direction.Down,
                                DeliveryPointType = (2 * x * y) % 2 == 0 ? DeliveryPointType.SingleUnit : DeliveryPointType.ProvidingGroup,
                                TotalEnergySupplied = 3 * x * y,

                                TimestampDetails = Enumerable.Range(0, 255)
                                    .Select(z => new ActivationControlDpTimestampDetail
                                    {
                                        Timestamp = deliveryDate.UtcDateTime.AddMinutes(15 * x).AddSeconds(4 * z),

                                        PowerMeasured = x * y * z,
                                        PowerBaseline = 2 * x * y * z,
                                        FcrCorrection = 3 * x * y * z,
                                        EnergySupplied = 6 * x * y * z,
                                    }).ToList(),
                            }).ToList(),

                    }).ToList()
            };

        private static IDeepDiff CreateDeepDiff()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.PersistEntity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .OnUpdate(cfg => cfg.CopyValues(x => x.AdditionalValueToCopy))
                .HasOne(x => x.SubEntity)
                .HasMany(x => x.SubEntities);
            diffConfiguration.PersistEntity<EntityLevel1>()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.Power, x.Price })
                .HasOne(x => x.SubEntity)
                .HasMany(x => x.SubEntities);
            diffConfiguration.PersistEntity<EntityLevel2>()
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.Value1, x.Value2 });
            var diff = diffConfiguration.CreateDeepDiff();
            return diff;
        }

        private static (EntityLevel0[] existingEntities, EntityLevel0[] newEntities) GenerateEntities(int n)
        {
            var now = DateTime.Now;
            var existingEntities = GenerateEntities(now, n).ToArray();
            var newEntities = GenerateEntities(now, n).ToArray();
            for (var entity0Index = 0; entity0Index < existingEntities.Length; entity0Index++)
            {
                var entity0 = existingEntities[entity0Index];
                if (entity0Index % 3 == 0)
                    entity0.StartsOn = entity0.StartsOn.AddMonths(1);
                if (entity0Index % 3 == 0)
                    entity0.Penalty = -5;
                for (var entity1Index = 0; entity1Index < entity0.SubEntities.Count; entity1Index++)
                {
                    var entity1 = entity0.SubEntities[entity1Index];
                    if (entity1Index % 4 == 0)
                        entity1.Timestamp = entity1.Timestamp.AddMonths(1);
                    if (entity1Index % 3 == 0)
                        entity1.Price = -5;
                    for (var entity2Index = 0; entity2Index < entity1.SubEntities.Count; entity2Index++)
                    {
                        var entity2 = entity1.SubEntities[entity2Index];
                        if (entity2Index % 5 == 0)
                            entity2.DeliveryPointEan = entity2.DeliveryPointEan + "_MOD";
                        if (entity2Index % 3 == 0)
                            entity2.Value1 = -5;
                    }
                }
            }

            return (existingEntities, newEntities);
        }

        private static IEnumerable<EntityLevel0> GenerateEntities(DateTime? now, int n)
        {
            return Enumerable.Range(0, n)
                .Select(x => new EntityLevel0
                {
                    Id = Guid.NewGuid(),

                    StartsOn = (now ?? DateTime.Now).AddMinutes(x),
                    Direction = Entities.Simple.Direction.Up,

                    RequestedPower = x,
                    Penalty = x,

                    SubEntity = new EntityLevel1
                    {
                        Id = Guid.NewGuid(),

                        Timestamp = now ?? DateTime.Now,

                        Power = x + 500,
                        Price = x * 500,

                        SubEntities = Enumerable.Range(0, 255)
                                .Select(z => new EntityLevel2
                                {
                                    Id = Guid.NewGuid(),

                                    DeliveryPointEan = $"DP_{x}_{500}_{z}",

                                    Value1 = x + 500 + z,
                                    Value2 = x * 500 * z,
                                }).ToList()
                    },

                    SubEntities = Enumerable.Range(0, 96)
                        .Select(y => new EntityLevel1
                        {
                            Id = Guid.NewGuid(),

                            Timestamp = (now ?? DateTime.Now).AddMinutes(x).AddSeconds(y),

                            Power = x + y,
                            Price = x * y,

                            SubEntity = new EntityLevel2
                            {
                                Id = Guid.NewGuid(),

                                DeliveryPointEan = $"DP_{x}_{y}_{500}",

                                Value1 = x + y + 500,
                                Value2 = x * y * 500,
                            },

                            SubEntities = Enumerable.Range(0, 255)
                                .Select(z => new EntityLevel2
                                {
                                    Id = Guid.NewGuid(),

                                    DeliveryPointEan = $"DP_{x}_{y}_{z}",

                                    Value1 = x + y + z,
                                    Value2 = x * y * z,
                                }).ToList()
                        }).ToList()
                }).ToList();
        }
    }
}
