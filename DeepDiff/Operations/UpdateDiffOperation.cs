namespace DeepDiff.Operations
{
    public class UpdateDiffOperation : DiffOperationBase
    {
        public string PropertyName { get; init; }
        public string ExistingValue { get; init; }
        public string NewValue { get; init; }
    }
}
