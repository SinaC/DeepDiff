using DeepDiff.Comparers;
using DeepDiff.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DeepDiff.UnitTest.Comparer
{
    internal class ComparerFactory<TEntity>
            where TEntity : class
    {
        public NaiveEqualityComparerByProperty<TEntity> CreateNaiveComparer<TKey>(Expression<Func<TEntity, TKey>> expression)
            => CreateNaiveComparer(expression, null!);

        public NaiveEqualityComparerByProperty<TEntity> CreateNaiveComparer<TKey>(Expression<Func<TEntity, TKey>> expression, IReadOnlyDictionary<Type, IEqualityComparer> typeSpecificComparers)
        {
            var properties = expression.GetSimplePropertyAccessList().Select(p => p.Single());
            return new NaiveEqualityComparerByProperty<TEntity>(properties, typeSpecificComparers);
        }

        public PrecompiledEqualityComparerByProperty<TEntity> CreatePrecompiledComparer<TKey>(Expression<Func<TEntity, TKey>> expression)
            => CreatePrecompiledComparer(expression, null!);

        public PrecompiledEqualityComparerByProperty<TEntity> CreatePrecompiledComparer<TKey>(Expression<Func<TEntity, TKey>> expression, IReadOnlyDictionary<Type, IEqualityComparer> typeSpecificComparers)
        {
            var properties = expression.GetSimplePropertyAccessList().Select(p => p.Single());
            return new PrecompiledEqualityComparerByProperty<TEntity>(properties, typeSpecificComparers);
        }
    }
}
