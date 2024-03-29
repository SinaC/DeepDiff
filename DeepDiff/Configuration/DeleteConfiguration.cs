﻿using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class DeleteConfiguration
    {
        public IList<SetValueConfiguration> SetValueConfigurations { get; private set; } = new List<SetValueConfiguration>();
        public bool GenerateOperations { get; private set; } = true;

        public SetValueConfiguration AddSetValueConfiguration(PropertyInfo destinationProperty, object value)
        {
            var config = new SetValueConfiguration(destinationProperty, value);
            SetValueConfigurations.Add(config);
            return config;
        }

        public void SetGenerationOperations(bool generationOperations)
        {
            GenerateOperations = generationOperations;
        }
    }
}
