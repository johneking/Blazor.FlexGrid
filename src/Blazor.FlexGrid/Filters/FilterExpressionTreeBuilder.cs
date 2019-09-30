using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Blazor.FlexGrid.Filters
{
    public class FilterExpressionTreeBuilder<TItem> : IFilterExpressionTreeBuilder<TItem> where TItem : class
    {
        public Expression<Func<TItem, bool>> BuildExpressionTree(IReadOnlyCollection<IFilterDefinition> filters)
        {
            if (!filters.Any())
            {
                return null;
            }

            var param = Expression.Parameter(typeof(TItem), "t");
            Expression filterExpressionTree;

            if (filters.Count == 1)
            {
                filterExpressionTree = FilterConverter.ConvertToExpression(filters.First(), param);
            }
            else
            {
                filterExpressionTree = FilterConverter.ConvertToExpression(filters.First(), param);
                foreach (var filter in filters.Skip(1))
                {
                    filterExpressionTree = Expression.AndAlso(filterExpressionTree, FilterConverter.ConvertToExpression(filter, param));
                }
            }

            return Expression.Lambda<Func<TItem, bool>>(filterExpressionTree, param);
        }
    }
}
