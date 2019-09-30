using Blazor.FlexGrid.DataSet;
using Blazor.FlexGrid.DataSet.Options;
using Microsoft.AspNetCore.Http.Extensions;
using System;

namespace Blazor.FlexGrid.DataAdapters
{
    /// <summary>
    /// Create <seealso cref="LazyTableDataSet{TItem}"/> 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class LazyLoadedTableDataAdapter<TItem> : BaseTableDataAdapter, ILazyLoadedTableDataAdapter where TItem : class
    {
        private readonly ILazyDataSetLoader<TItem> _lazyDataSetLoader;
        private readonly ILazyGroupableDataSetLoader<TItem> _lazyGroupableDataSetLoader;
        private readonly ILazyDataSetItemManipulator<TItem> _lazyDataSetItemSaver;

        public override Type UnderlyingTypeOfItem => typeof(TItem);

        public Action<QueryBuilder> AddRequestParamsAction { get; set; }

        public LazyLoadedTableDataAdapter(
            ILazyDataSetLoader<TItem> lazyDataSetLoader,
            ILazyGroupableDataSetLoader<TItem> lazyGroupableDataSetLoader,
            ILazyDataSetItemManipulator<TItem> lazyDataSetItemSaver)
        {
            _lazyDataSetLoader = lazyDataSetLoader ?? throw new ArgumentNullException(nameof(lazyDataSetLoader));
            _lazyGroupableDataSetLoader = lazyGroupableDataSetLoader ?? throw new ArgumentNullException(nameof(lazyGroupableDataSetLoader));
            _lazyDataSetItemSaver = lazyDataSetItemSaver ?? throw new ArgumentNullException(nameof(lazyDataSetItemSaver));
        }

        public override ITableDataSet GetTableDataSet(Action<TableDataSetOptions> configureDataSet)
        {
            var tableDataSetOptions = new TableDataSetOptions();
            configureDataSet?.Invoke(tableDataSetOptions);

            var lazyLoadingOptions = tableDataSetOptions.LazyLoadingOptions;
            AddRequestParamsAction?.Invoke(lazyLoadingOptions.RequestParams);

            var tableDataSet = new LazyTableDataSet<TItem>(_lazyDataSetLoader, _lazyGroupableDataSetLoader, _lazyDataSetItemSaver)
            {
                LazyLoadingOptions = lazyLoadingOptions,
                PageableOptions = tableDataSetOptions.PageableOptions,
                SortingOptions = tableDataSetOptions.SortingOptions,
                GroupingOptions = tableDataSetOptions.GroupingOptions,
                GridViewEvents = tableDataSetOptions.GridViewEvents
            };

            return tableDataSet;
        }

        public override object Clone()
            => new LazyLoadedTableDataAdapter<TItem>(_lazyDataSetLoader, _lazyGroupableDataSetLoader, _lazyDataSetItemSaver);
    }

    /// <summary>
    /// Only for type check purposes
    /// </summary>
    internal interface ILazyLoadedTableDataAdapter
    {
        Action<QueryBuilder> AddRequestParamsAction { set; }
    }
}
