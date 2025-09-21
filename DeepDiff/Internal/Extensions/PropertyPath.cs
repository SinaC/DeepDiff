using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DeepDiff.Internal.Extensions
{
    internal sealed class PropertyPath : IEnumerable<PropertyInfo>
    {
        // Note: This class is currently immutable. If you make it mutable then you
        // must ensure that instances are cloned when cloning the DbModelBuilder.
        private static PropertyPath InternalEmpty { get; } = new PropertyPath();

        private List<PropertyInfo> Components { get; } = new List<PropertyInfo>();

        public PropertyPath(IEnumerable<PropertyInfo> components)
        {
            Components.AddRange(components);
        }

        public PropertyPath(PropertyInfo component)
        {
            Components.Add(component);
        }

        private PropertyPath()
        {
        }

        public int Count => Components.Count;

        public static PropertyPath Empty => InternalEmpty;

        public PropertyInfo this[int index] => Components[index];

        public override string ToString()
        {
            var propertyPathName = new StringBuilder();

            foreach (var pi in Components)
            {
                propertyPathName.Append(pi.Name);
                propertyPathName.Append('.');
            }

            return propertyPathName.ToString(0, propertyPathName.Length - 1);
        }

        #region Equality Members

        public bool Equals(PropertyPath other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Components.SequenceEqual(other.Components, (p1, p2) => p1!.IsSameAs(p2!));
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(PropertyPath))
            {
                return false;
            }

            return Equals((PropertyPath)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Components.Aggregate(
                    0,
                    (t, n) => t ^ n.DeclaringType!.GetHashCode() * n.Name.GetHashCode() * 397);
            }
        }

        public static bool operator ==(PropertyPath left, PropertyPath right)
            => Equals(left, right);

        public static bool operator !=(PropertyPath left, PropertyPath right)
            => !Equals(left, right);

        #endregion

        #region IEnumerable Members

        IEnumerator<PropertyInfo> IEnumerable<PropertyInfo>.GetEnumerator() 
            => Components.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Components.GetEnumerator();

        #endregion
    }
}