﻿using System.Reflection;

namespace DeepDiff.Comparers
{
    internal class CompareByPropertyResultDetail
    {
        public PropertyInfo PropertyInfo { get; init; }
        public object OldValue { get; init; }
        public object NewValue { get; init; }
    }
}
