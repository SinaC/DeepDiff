using System.Reflection;

namespace DeepDiff.Comparers
{
    internal class ComparerByPropertyResult
    {
        public PropertyInfo PropertyInfo { get; init; }
        public object OldValue { get; init; }
        public object NewValue { get; init; }
    }
}
