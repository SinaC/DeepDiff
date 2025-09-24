using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DeepDiff.Internal
{
    internal class PropertyInfoExt
    {
        public PropertyInfo PropertyInfo { get; }
        public string Name => PropertyInfo.Name;
        public Type PropertyType => PropertyInfo.PropertyType;
        private Func<object?, object?> Getter { get; }
        private Action<object?, object?> Setter { get; }


        public PropertyInfoExt(Type type, PropertyInfo propertyInfo)
        {
            // Getter dynamic method the signature would be :
            // object Get(object thisReference)
            // { return ((TestClass)thisReference).Prop; }
            var dmGet = new DynamicMethod("Get", typeof(object), new Type[] { typeof(object), });
            var ilGet = dmGet.GetILGenerator();
            // Load first argument to the stack
            ilGet.Emit(OpCodes.Ldarg_0);
            // Cast the object on the stack to the apropriate type
            ilGet.Emit(OpCodes.Castclass, type);
            // Call the getter method passing the object on teh stack as the this reference
            ilGet.Emit(OpCodes.Callvirt, propertyInfo.GetGetMethod()!);
            // If the property type is a value type (int/DateTime/..)
            // box the value so we can return it
            if (propertyInfo.PropertyType.IsValueType)
            {
                ilGet.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }
            // Return from the method
            ilGet.Emit(OpCodes.Ret);

            // Setter dynamic method the signature would be :
            // object Set(object thisReference, object propValue)
            // { return ((TestClass)thisReference).Prop = (PropType)propValue; }
            var dmSet = new DynamicMethod("Set", typeof(void), new Type[] { typeof(object), typeof(object) });
            var ilSet = dmSet.GetILGenerator();
            // Load first argument to the stack and cast it
            ilSet.Emit(OpCodes.Ldarg_0);
            ilSet.Emit(OpCodes.Castclass, type);

            // Load secons argument to the stack and cast it or unbox it
            ilSet.Emit(OpCodes.Ldarg_1);
            if (propertyInfo.PropertyType.IsValueType)
            {
                ilSet.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
            }
            else
            {
                ilSet.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
            }
            // Call Setter method and return
            ilSet.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod()!);
            ilSet.Emit(OpCodes.Ret);

            //
            PropertyInfo = propertyInfo;
            Getter = (Func<object?, object?>)dmGet.CreateDelegate(typeof(Func<object?, object?>));
            Setter = (Action<object?, object?>)dmSet.CreateDelegate(typeof(Action<object?, object?>));
        }

        // use optimized getter and setter
        public object? GetValue(object? obj) => Getter(obj);
        public void SetValue(object? obj, object? value) => Setter(obj, value);

        // override Equals and GetHashCode so we can use PropertyInfoExt in Linq .Distinct(), .Contains(), ...
        // two PropertyInfoExt are equals if they wrap the same PropertyInfo
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

            if (obj is not PropertyInfoExt propertyInfoExt)
            {
                return false;
            }

            return PropertyInfo.Equals(propertyInfoExt.PropertyInfo);
        }

        public override int GetHashCode()
        {
            return PropertyInfo.GetHashCode();
        }
    }
}
