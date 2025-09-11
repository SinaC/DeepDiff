﻿using System.Reflection;

namespace DeepDiff.Internal.Comparers
{
    internal sealed class CompareByPropertyResultDetail
    {
        public PropertyInfo PropertyInfo { get; init; } = null!;
        public object? OldValue { get; init; } = null!;
        public object? NewValue { get; init; } = null!;
    }
}
