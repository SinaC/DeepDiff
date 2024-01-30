using System.Reflection;

namespace DeepDiff.Comparers
{
    internal class CompareByPropertyResultDetail
    {
        public CompareByPropertyResultDetail()
        {
        }

        public CompareByPropertyResultDetail(PropertyInfo propertyInfo, object oldValue, object newValue)
        {
            PropertyInfo = propertyInfo;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public PropertyInfo PropertyInfo { get; init; }
        public object OldValue { get; init; }
        public object NewValue { get; init; }
    }
}
