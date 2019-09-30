using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Blazor.FlexGrid.DataSet.Options;

namespace Blazor.FlexGrid.DataSet
{
	public static class QueryableFilterHelpers
	{
		public static IQueryable<GroupItem<TItem>> ApplySortingToGroupedQueryable<TItem>(this IQueryable<GroupItem<TItem>> queryable, ISortingOptions sortingOptions, IGroupingOptions groupingOptions)
		{
			if (string.IsNullOrEmpty(sortingOptions?.SortExpression))
			{
				return queryable;
			}
			if (sortingOptions.SortExpression != groupingOptions.GroupedProperty.Name)
			{
				queryable = queryable.Select(x => new GroupItem<TItem>(x.Key,
					sortingOptions.SortDescending
						? x.AsQueryable().OrderByDescending(sortingOptions.SortExpression)
						: x.AsQueryable().OrderBy(sortingOptions.SortExpression)));
				return queryable;
			}

			return sortingOptions.SortDescending
				? queryable.OrderByDescending(x => x.Key)
				: queryable.OrderBy(x => x.Key);
		}

		public static IQueryable<TItem> GetSourceWithoutDeleted<TItem>(this IQueryable<TItem> source, Expression<Func<TItem, bool>> filterExpression, HashSet<object> deleted)
		{
			var filteredSource = filterExpression is null ? source : source.Where(filterExpression);
			var sourceWithoutDeleted = ApplyDeletedConditionToQueryable(filteredSource, deleted);
			return sourceWithoutDeleted;
		}

		public static IQueryable<GroupItem<TItem>> ApplyFiltersWithGroupingToQueryable<TItem>(this IQueryable<TItem> source, ISortingOptions sortingOptions, IGroupingOptions groupingOptions, IPagingOptions pagingOptions)
		{
			var groupedItemsQueryable = ApplyGroupingToQueryable(source, groupingOptions.GroupedProperty.Name);
			groupedItemsQueryable = ApplySortingToGroupedQueryable(groupedItemsQueryable, sortingOptions, groupingOptions);
			pagingOptions.TotalItemsCount = groupedItemsQueryable.Count();
			groupedItemsQueryable = ApplyPagingToQueryable(groupedItemsQueryable, pagingOptions);
			return groupedItemsQueryable;
		}

        public static IQueryable<GroupItem<TItem>> ApplyGroupingToQueryable<TItem>(this IQueryable<TItem> source, string groupPropertyName)
		{
			return source.GroupBy(groupPropertyName, "it")
				.Select<GroupItem<TItem>>(ParsingConfig.Default, "new (it.Key as Key, it as Items)");
		}

        public static IQueryable<T> ApplyPagingToQueryable<T>(this IQueryable<T> queryable, IPagingOptions pageableOptions)
		{
			return pageableOptions != null && pageableOptions.PageSize > 0
				? queryable.Skip(pageableOptions.PageSize * pageableOptions.CurrentPage)
					.Take(pageableOptions.PageSize)
				: queryable;
		}

		public static IQueryable<TItem> ApplySortingToQueryable<TItem>(this IQueryable<TItem> queryable, ISortingOptions sortingOptions)
		{
			if (string.IsNullOrEmpty(sortingOptions?.SortExpression))
			{
				return queryable;
			}

			return sortingOptions.SortDescending
				? queryable.OrderByDescending(sortingOptions.SortExpression)
				: queryable.OrderBy(sortingOptions.SortExpression);
		}

		public static IQueryable<TItem> ApplyDeletedConditionToQueryable<TItem>(this IQueryable<TItem> queryable, HashSet<object> deleted)
			=> !deleted.Any()
				? queryable
				: queryable.Where(i => !deleted.Contains(i));
	}
}