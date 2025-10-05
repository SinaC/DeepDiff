using DeepDiff.Configuration;
using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.ActivationRemuneration;
using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Extensions;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Profiles
{
    public class ActivationRemunerationDiffProfile : DiffProfile
    {
        public ActivationRemunerationDiffProfile()
        {
            CreateConfiguration<ActivationRemuneration>()
                .CreateAuditEntity<ActivationRemuneration, int>()
                .HasKey(x => new { x.Day, x.ContractReference })
                .HasMany(x => x.ActivationRemunerationDetails);

            CreateConfiguration<ActivationRemunerationDetail>()
                .PersistEntity()
                .HasKey(x => x.StartsOn)
                .HasValues(x => new { x.IsProrata, x.EnergyRequested, x.EnergyRequestedSendToSupplier, x.EnergyRequestedRatio, x.IsEnergyRequestedRatioIncoherent })
                .OnUpdate(cfg => cfg.CopyValues(x => new { x.Status, x.SupplierStatus }))
                .HasMany(x => x.ActivationRemunerationDirectionDetails)
                .IgnoreAudit()
                .Ignore(x => new { x.ActivationRemunerationId, x.ActivationRemuneration, x.InternalComment, x.TsoComment, x.SupplierComment });

            CreateConfiguration<ActivationRemunerationDirectionDetail>()
                .PersistEntity()
                .HasKey(x => x.Direction)
                .HasValues(x => new { x.EnergyPrice, x.EnergyRequested, x.EnergyRequestedForRedispatching, x.Remuneration, x.QualityCheck })
                .OnUpdate(cfg => cfg.CopyValues(x => new { x.EnergyRequestedOverride, x.EnergyPriceOverride }))
                .HasMany(x => x.ActivationRemunerationBids)
                .IgnoreAudit()
                .Ignore(x => new { x.ActivationRemunerationId, x.StartsOn, x.ActivationRemunerationDetail });

            CreateConfiguration<ActivationRemunerationBid>()
                .PersistEntity()
                .HasKey(x => new { x.BidGroupId, x.DeliveryPointEan })
                .HasValues(x => new { x.DeliveryPointName, x.EnergyRequested, x.EnergyRequestedForRedispatching, x.EnergyPrice, x.BidPrice, x.Remuneration })
                .HasMany(x => x.ActivationRemunerationBidDetails)
                .Ignore(x => new { x.ActivationRemunerationId, x.StartsOn, x.Direction, x.ActivationRemunerationDirectionDetail });

            CreateConfiguration<ActivationRemunerationBidDetail>()
                .PersistEntity()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.IsConnectedToPicasso, x.IsProrata, x.VolumeRequested, x.VolumeRequestedForRedispatching, x.MarginalPrice, x.EnergyPrice, x.Remuneration })
                .Ignore(x => new { x.ActivationRemunerationId, x.StartsOn, x.Direction, x.BidGroupId, x.ActivationRemunerationBid });
        }
    }
}
