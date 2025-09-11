using System;

namespace DeepDiff.Exceptions
{
    public class CompareWithoutOperationListenerException : Exception
    {
        public CompareWithoutOperationListenerException()
            : base("Comparing without an operation listener is not supported. Please provide an IOperationListener implementation to capture the differences.")
        {
        }
    }
}
