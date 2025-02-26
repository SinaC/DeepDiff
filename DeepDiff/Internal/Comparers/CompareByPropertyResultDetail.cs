using System.Reflection;

namespace DeepDiff.Internal.Comparers
{
    internal sealed class CompareByPropertyResultDetail
    {
        public PropertyInfo PropertyInfo { get; init; }
        public object OldValue { get; init; }
        public object NewValue { get; init; }
    }
}
