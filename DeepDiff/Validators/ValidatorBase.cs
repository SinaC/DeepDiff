using DeepDiff.Configuration;
using System.Collections.Generic;
using System;

namespace DeepDiff.Validators
{
    internal abstract class ValidatorBase
    {
        public abstract IEnumerable<Exception> Validate(Type entityType, DiffEntityConfiguration diffEntityConfiguration, IReadOnlyDictionary<Type, DiffEntityConfiguration> diffEntityConfigurationByTypes);

        protected static string NameOf<T>()
            => typeof(T).Name.Replace("Configuration", string.Empty);
    }
}
