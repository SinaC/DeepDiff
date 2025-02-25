using System;

namespace DeepDiff.Configuration
{
    [Flags]
    public enum GenerateOperations
    {
        None = 0x00,

        Insert = 0x01,
        Delete = 0x02,
        Update = 0x04,

        All = Insert | Delete | Update,
    }
}
