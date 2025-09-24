using System;
using System.Reflection;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class NavigationManyConfiguration : NavigationConfigurationBase
    {
        public bool UseDerivedTypes { get; private set; }

        public NavigationManyConfiguration(Type entityType, PropertyInfo navigationProperty, Type navigationChildType)
            : base(entityType, navigationProperty, navigationChildType)
        {
            if (NavigationChildType.IsAbstract) // we force UseDerivedTypes if child entity type is abstract
                UseDerivedTypes = true;
        }

        public void SetUseDerivedTypes(bool useDerivedTypes)
        {
            if (!NavigationChildType.IsAbstract) // if IsAbstract, UseDerivedTypes cannot be modified
                UseDerivedTypes = useDerivedTypes;
        }
    }
}