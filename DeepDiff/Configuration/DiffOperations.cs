using System;

namespace DeepDiff.Configuration
{
    [Flags]
    public enum DiffOperations
    {
        None = 0x00,

        Insert = 0x01,
        Delete = 0x02,

        UpdateValue = 0x10, // value defined in HasValues updated
        UpdateSetValue = 0x10, // value defined in SetValue updated
        UpdateCopyValue = 0x40, // value defined in CopyValues updated

        Update = UpdateValue | UpdateSetValue | UpdateCopyValue,

        All = Insert | Delete | Update,
    }
}
