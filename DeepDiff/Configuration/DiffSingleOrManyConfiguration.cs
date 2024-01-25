namespace DeepDiff.Configuration
{
    internal abstract class DiffSingleOrManyConfiguration
    {
        public bool UseHashtable { get; protected set; } = true;
        public int HashtableThreshold { get; protected set; } = 15;
        public bool OnUpdateEvenIfModificationsDetectedOnlyInNestedLevel { get; protected set; } = false;
        public bool GenerateOperations { get; protected set; } = true;
        public bool UsePrecompiledEqualityComparer { get; protected set; } = true;
        public int? DecimalPrecision { get; protected set; } = null; // if null, no specific compare
    }
}
