using System.Reflection;

namespace DeepDiff.Configuration
{
    public interface IDeepDiffConfiguration
    {
        /// <summary>
        /// Creates a new instance of <see cref="IEntityConfiguration{TEntity}"/> for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exceptions.AbstractEntityConfigurationException"/>
        /// <exception cref="Exceptions.DuplicateEntityConfigurationException"/>
        IEntityConfiguration<TEntity> Entity<TEntity>()
            where TEntity : class;

        /// <summary>
        /// Adds a profile to the configuration.
        /// </summary>
        /// <typeparam name="TProfile"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exceptions.AbstractEntityConfigurationException"/>
        /// <exception cref="Exceptions.DuplicateEntityConfigurationException"/>
        IDeepDiffConfiguration AddProfile<TProfile>()
            where TProfile : DiffProfile;

        /// <summary>
        /// Adds a profile to the configuration.
        /// </summary>
        /// <param name="diffProfile"></param>
        /// <returns></returns>
        /// <exception cref="Exceptions.AbstractEntityConfigurationException"/>
        /// <exception cref="Exceptions.DuplicateEntityConfigurationException"/>
        IDeepDiffConfiguration AddProfile(DiffProfile diffProfile);

        /// <summary>
        /// Adds profiles from the specified assemblies to the configuration.
        /// </summary>
        /// <param name="assembliesToScan"></param>
        /// <returns></returns>
        /// <exception cref="Exceptions.AbstractEntityConfigurationException"/>
        /// <exception cref="Exceptions.DuplicateEntityConfigurationException"/>
        IDeepDiffConfiguration AddProfiles(params Assembly[] assembliesToScan);

        /// <summary>
        /// Creates a new instance of <see cref="IDeepDiff"/> with the current configuration.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.AggregateException"/>
        /// <exception cref="Exceptions.AlreadyDefinedPropertyException"/>
        /// <exception cref="Exceptions.DuplicatePropertyConfigurationException"/>
        /// <exception cref="Exceptions.EmptyConfigurationException"/>
        /// <exception cref="Exceptions.MissingKeyConfigurationException"/>
        /// <exception cref="Exceptions.MissingNavigationOneChildConfigurationException"/>
        /// <exception cref="Exceptions.MissingNavigationManyAbstractChildConfigurationException"/>
        /// <exception cref="Exceptions.MissingNavigationManyChildConfigurationException"/>
        /// <exception cref="Exceptions.NoKeyAndHasKeyConfigurationException"/>
        /// <exception cref="Exceptions.NoKeyButFoundInNavigationManyConfigurationException"/>
        /// <exception cref="Exceptions.InvalidNavigationOneChildTypeConfigurationException"/>
        IDeepDiff CreateDeepDiff();

        /// <summary>
        /// Validates the configuration. Throw an exception if the configuration is invalid.
        /// </summary>
        /// <exception cref="System.AggregateException"/>
        /// <exception cref="Exceptions.AlreadyDefinedPropertyException"/>
        /// <exception cref="Exceptions.DuplicatePropertyConfigurationException"/>
        /// <exception cref="Exceptions.EmptyConfigurationException"/>
        /// <exception cref="Exceptions.MissingKeyConfigurationException"/>
        /// <exception cref="Exceptions.MissingNavigationOneChildConfigurationException"/>
        /// <exception cref="Exceptions.MissingNavigationManyAbstractChildConfigurationException"/>
        /// <exception cref="Exceptions.MissingNavigationManyChildConfigurationException"/>
        /// <exception cref="Exceptions.NoKeyAndHasKeyConfigurationException"/>
        /// <exception cref="Exceptions.NoKeyButFoundInNavigationManyConfigurationException"/>
        /// <exception cref="Exceptions.InvalidNavigationOneChildTypeConfigurationException"/>
        void ValidateConfiguration();

        /// <summary>
        /// Validates if every properties are referenced in the configuration. Throw an exception if a property is not referenced.
        /// </summary>
        /// <exception cref="System.AggregateException"/>
        /// <exception cref="Exceptions.PropertyNotReferenceInConfigurationException"/>
        void ValidateIfEveryPropertiesAreReferenced();
    }
}