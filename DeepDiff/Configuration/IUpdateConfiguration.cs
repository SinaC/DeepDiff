using System;
using System.Linq.Expressions;

namespace DeepDiff.Configuration
{
    /// <summary>
    /// Configuration for update operations.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IUpdateConfiguration<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// When an update is detected, overwrite property with a specific value
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="destinationMember"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        IUpdateConfiguration<TEntity> SetValue<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember? value);
        /// <summary>
        /// When an update is detected, copy the value(s) from the new entity to the existing entity
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="copyValuesExpression"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        IUpdateConfiguration<TEntity> CopyValues<TValue>(Expression<Func<TEntity, TValue>> copyValuesExpression);
    }
}
