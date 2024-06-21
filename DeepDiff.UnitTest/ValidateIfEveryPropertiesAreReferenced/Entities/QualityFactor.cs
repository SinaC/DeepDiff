using System;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities
{
    [Flags]
    public enum QualityFactor
    {
        None = 0,
        Invalid = 1,
        Missing = 2
    }
}
