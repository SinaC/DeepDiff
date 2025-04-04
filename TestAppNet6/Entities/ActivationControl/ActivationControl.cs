using System.Diagnostics;

namespace TestAppNet6.Entities.ActivationControl;

[DebuggerDisplay("{DebuggerDisplay, nq}")]
public class ActivationControl : UpdateAuditEntity
{
    public string ContractReference { get; set; } = null!;
    public Date Day { get; set; }

    public decimal TotalEnergyRequested { get; set; }
    public decimal TotalDiscrepancy { get; set; }
    public decimal TotalEnergyToBeSupplied { get; set; }
    public decimal FailedPercentage { get; set; }
    public int IsMeasurementExcludedCount { get; set; }
    public int IsJumpExcludedCount { get; set; }

    public ActivationControlStatus Status { get; set; }
    public string InternalComment { get; set; } = null!;
    public string TsoComment { get; set; } = null!;

    // one-to-many
    public List<ActivationControlDetail> ActivationControlDetails { get; set; } = null!;

    private string DebuggerDisplay => $"{ContractReference} {Day} TER:{TotalEnergyRequested} TD:{TotalDiscrepancy} TETBS:{TotalEnergyToBeSupplied} FP:{FailedPercentage} MEC:{IsMeasurementExcludedCount} JEC:{IsJumpExcludedCount} {Id}";
}
