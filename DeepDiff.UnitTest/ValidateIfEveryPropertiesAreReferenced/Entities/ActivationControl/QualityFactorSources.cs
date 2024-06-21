using System;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.ActivationControl
{
    [Flags]
    public enum QualityFactorSources : short
    {
        None                    = 0x0000,
        FlexHub                 = 0x0001,
        CCPowerMeasured         = 0x0002,
        CCPowerBaseline         = 0x0004,
        CCAvailableSec          = 0x0008,
        EmsExtractPowerMeasured = 0x0010,
        EmsExtractPowerBaseline = 0x0020,
        EmsExtractAvailableSec  = 0x0040,
    }
}
