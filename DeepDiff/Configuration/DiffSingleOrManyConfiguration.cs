namespace DeepDiff.Configuration
{
    internal abstract class DiffSingleOrManyConfiguration
    {
        public bool UseHashtable { get; protected set; } = true;
        public int HashtableThreshold { get; protected set; } = 15;
        public bool OnUpdateEvenIfModificatiosnDetectedOnlyInNestedLevel { get; protected set; } = false;
    }
}
