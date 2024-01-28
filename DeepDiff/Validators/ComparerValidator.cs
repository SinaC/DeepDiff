using DeepDiff.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepDiff.Validators
{
    internal sealed class ComparerValidator : ValidatorBase
    {
        // TODO: check if equality comparer are able to convert type or property
        public override IEnumerable<Exception> Validate(Type entityType, DiffEntityConfiguration diffEntityConfiguration, IReadOnlyDictionary<Type, DiffEntityConfiguration> diffEntityConfigurationByTypes)
        {
            return Enumerable.Empty<Exception>();
        }
    }
}
