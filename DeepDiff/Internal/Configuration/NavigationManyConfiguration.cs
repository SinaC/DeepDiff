using System.Reflection;
using System;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class NavigationManyConfiguration : NavigationConfigurationBase
    {
        public bool UseDerivedTypes { get; private set; }

        public NavigationManyConfiguration(PropertyInfo navigationProperty, Type navigationChildType)
            : base(navigationProperty, navigationChildType)
        {
            if (NavigationChildType.IsAbstract) // we force UseDerivedTypes if child entity type is abstract
                UseDerivedTypes = true;
        }

        public void SetUseDerivedTypes(bool useDerivedTypes)
        {
            if (!NavigationChildType.IsAbstract) // when IsAbstract, UseDerivedTypes is been forced to true and cannot be modified
                UseDerivedTypes = useDerivedTypes;
        }
    }
}