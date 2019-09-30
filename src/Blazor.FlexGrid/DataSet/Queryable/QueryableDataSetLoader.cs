using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor.FlexGrid.DataSet.Http;
using Blazor.FlexGrid.Filters;
using Microsoft.Extensions.Logging;

namespace Blazor.FlexGrid.DataSet.Queryable
{
	public class QueryableDataSetLoader<TItem> : ILazyDataSetLoader<TItem> where TItem : class
	{
		private readonly ILogger<QueryableDataSetLoader<TItem>> _logger;
		private readonly Func<IQueryable<TItem>> _queryableSource;
		private readonly IFilterExpressionTreeBuilder<TItem> _filterExpressionTreeBuilder;
		public QueryableDataSetLoader(ILogger<QueryableDataSetLoader<TItem>> logger, Func<IQueryable<TItem>> queryableSource, IFilterExpressionTreeBuilder<TItem> filterExpressionTreeBuilder)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_queryableSource = queryableSource;
			_filterExpressionTreeBuilder = filterExpressionTreeBuilder;
		}

		public Task<LazyLoadingDataSetResult<TItem>> GetTablePageData(
			RequestOptions requestOptions,
			IReadOnlyCollection<IFilterDefinition> filterDefinitions = null)
		{
			try
			{
				var queryable = _queryableSource();
				var totalItems = queryable.Count();
				if (filterDefinitions != null && filterDefinitions.Any())
				{
					queryable = queryable.Where(_filterExpressionTreeBuilder.BuildExpressionTree(filterDefinitions));
				}
				var items = queryable
					.ApplySortingToQueryable(requestOptions.SortingOptions)
					.ApplyPagingToQueryable(requestOptions.PageableOptions)
					.ToList();
				var result = new LazyLoadingDataSetResult<TItem> { Items = items, TotalCount = totalItems };
				return Task.FromResult(result);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during fetching data. Ex: {ex}");

				var emptyResult = new LazyLoadingDataSetResult<TItem>
				{
					Items = Enumerable.Empty<TItem>().ToList()
				};

				return Task.FromResult(emptyResult);
			}
		}
	}
}