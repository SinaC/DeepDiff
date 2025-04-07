namespace DeepDiff.Configuration
{
    /// <summary>
    /// Defines a generic listener that can be used instead of the IOperationListener if an implementation by Entity is needed
    /// </summary>
    public interface IOperationListener<T> : IOperationListener
        where T : class
    {
    }
}
