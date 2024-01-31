using DeepDiff.Configuration;
using System.Collections.Generic;
using System;

namespace DeepDiff.Validators
{
    internal abstract class ValidatorBase
    {
        public abstract IEnumerable<Exception> Validate(Type entityType, EntityConfiguration entityConfiguration, IReadOnlyDictionary<Type, EntityConfiguration> entityConfigurationByTypes);

        protected static string NameOf<T>()
            => typeof(T).Name.Replace("Configuration", string.Empty);
    }
}
