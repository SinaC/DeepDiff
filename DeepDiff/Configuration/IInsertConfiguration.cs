using System;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    /// <summary>
    /// Configuration for insert operations.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IInsertConfiguration<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// When an insert is detected, overwrite property with a specific value
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="destinationMember"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        IInsertConfiguration<TEntity> SetValue<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember? value);
    }
}
