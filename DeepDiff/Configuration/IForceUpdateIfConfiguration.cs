using System;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    /// <summary>
    /// Configuration to defines additional criteria to detect an update even if no update is detected.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IForceUpdateIfConfiguration<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Trigger an update when a nested entity is modified.
        /// </summary>
        /// <returns></returns>
        IForceUpdateIfConfiguration<TEntity> NestedEntitiesModified();

        /// <summary>
        /// Trigger an update when an equality condition is met.
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="compareToMember"></param>
        /// <param name="compareToValue"></param>
        /// <returns></returns>
        IForceUpdateIfConfiguration<TEntity> Equals<TMember>(Expression<Func<TEntity, TMember>> compareToMember, TMember? compareToValue);
    }
}
