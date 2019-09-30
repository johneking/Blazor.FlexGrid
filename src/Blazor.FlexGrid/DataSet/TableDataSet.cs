using Blazor.FlexGrid.Components.Configuration.ValueFormatters;
using Blazor.FlexGrid.Components.Events;
using Blazor.FlexGrid.DataSet.Options;
using Blazor.FlexGrid.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blazor.FlexGrid.DataSet
{
    public class TableDataSet<TItem> : ITableDataSet, IBaseTableDataSet<TItem> where TItem : class
    {
        private readonly IFilterExpressionTreeBuilder<TItem> _filterExpressionTreeBuilder;

        private readonly IQueryable<TItem> _source;
        private Expression<Func<TItem, bool>> _filterExpression;
        private readonly HashSet<object> _selectedItems;
        private readonly HashSet<object> _deletedItems;

        public IPagingOptions PageableOptions { get; set; } = new PageableOptions();

        public ISortingOptions SortingOptions { get; set; } = new SortingOptions();

        public IRowEditOptions RowEditOptions { get; set; } = new RowEditOptions();

        public IGroupingOptions GroupingOptions { get; set; } = new GroupingOptions();

        public GridViewEvents GridViewEvents { get; set; } = new GridViewEvents();

        public bool FilterIsApplied => _filterExpression != null;

        /// <summary>
        /// Gets or sets the items for the current page.
        /// </summary>
        public IList<TItem> Items { get; set; } = new List<TItem>();

        IList IBaseTableDataSet.Items => Items is List<TItem> list ? list : Items.ToList();

        public IList<GroupItem> GroupedItems { get; private set; } = new List<GroupItem>();

        public TableDataSet(
            IQueryable<TItem> source,
            IFilterExpressionTreeBuilder<TItem> filterExpressionTreeBuilder)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _filterExpressionTreeBuilder = filterExpressionTreeBuilder ?? throw new ArgumentNullException(nameof(filterExpressionTreeBuilder));
            _selectedItems = new HashSet<object>();
            _deletedItems = new HashSet<object>();
        }

        public Task GoToPage(int index)
        {
            PageableOptions.CurrentPage = index;
            ApplyFiltersToQueryableSource(_source, _filterExpression, _deletedItems);

            return Task.CompletedTask;
        }

        public Task SetSortExpression(string expression)
        {
            if (SortingOptions.SortExpression != expression)
            {
                SortingOptions.SortExpression = expression;
                SortingOptions.SortDescending = false;
            }
            else
            {
                SortingOptions.SortDescending = !SortingOptions.SortDescending;
            }

            return GoToPage(0);
        }

        public Task ApplyFilters(IReadOnlyCollection<IFilterDefinition> filters)
        {
            if (!filters.Any())
            {
                _filterExpression = null;
                return GoToPage(0);
            }

            PageableOptions.CurrentPage = 0;
            _filterExpression = _filterExpressionTreeBuilder.BuildExpressionTree(filters);

            return GoToPage(0);
        }

        public void ToggleRowItem(object item)
        {
            if (ItemIsSelected(item))
            {
                _selectedItems.Remove(item);
                return;
            }

            _selectedItems.Add(item);
        }

        public bool ItemIsSelected(object item)
            => _selectedItems.Contains(item);

        public void StartEditItem(object item)
        {
            if (item != null)
            {
                RowEditOptions.ItemInEditMode = item;
            }
        }

        public void EditItemProperty(string propertyName, object propertyValue)
            => RowEditOptions.AddNewValue(propertyName, propertyValue);

        public Task<bool> SaveItem(ITypePropertyAccessor propertyValueAccessor)
        {
            try
            {
                foreach (var newValue in RowEditOptions.UpdatedValues)
                {
                    propertyValueAccessor.SetValue(RowEditOptions.ItemInEditMode, newValue.Key, newValue.Value);
                }

                GridViewEvents.SaveOperationFinished?.Invoke(new SaveResultArgs { ItemSucessfullySaved = true, Item = RowEditOptions.ItemInEditMode });

                return Task.FromResult(true);
            }
            catch (Exception)
            {
                GridViewEvents.SaveOperationFinished?.Invoke(new SaveResultArgs { ItemSucessfullySaved = false });
                return Task.FromResult(false);
            }
            finally
            {
                RowEditOptions.ItemInEditMode = EmptyDataSetItem.Instance;
            }
        }

        public async Task<bool> DeleteItem(object item)
        {
            var removeResult = Items.Remove((TItem)item);
            if (removeResult)
            {
                PageableOptions.TotalItemsCount--;
                _deletedItems.Add(item);
                GridViewEvents.DeleteOperationFinished?.Invoke(new DeleteResultArgs { ItemSuccesfullyDeleted = true, Item = item });
                await GoToPage(PageableOptions.CurrentPage);
            }
            else
            {
                GridViewEvents.DeleteOperationFinished?.Invoke(new DeleteResultArgs { ItemSuccesfullyDeleted = false, Item = item });
            }

            return removeResult;
        }

        public void CancelEdit()
            => RowEditOptions.ItemInEditMode = EmptyDataSetItem.Instance;

        public void ToggleGroupRow(object groupItemKey)
            => GroupedItems.ToggleGroup(groupItemKey);


        private void ApplyFiltersToQueryableSource(IQueryable<TItem> source, Expression<Func<TItem, bool>> filterExpression, HashSet<object> deleted)
        {
	        var sourceWithoutDeleted = source.GetSourceWithoutDeleted(filterExpression, deleted);
	        if (!GroupingOptions.IsGroupingActive)
	        {
		        PageableOptions.TotalItemsCount = sourceWithoutDeleted.Count();
		        Items = ApplyFiltersToQueryable(sourceWithoutDeleted).ToList();
	        }
	        else
	        {
		        var newGroupedItems = ApplyFiltersWithGroupingToQueryable(sourceWithoutDeleted).OfType<GroupItem>().ToList();
		        newGroupedItems.PreserveGroupCollapsing(GroupedItems);
		        GroupedItems = newGroupedItems;
	        }
        }

        private IQueryable<GroupItem<TItem>> ApplyFiltersWithGroupingToQueryable(IQueryable<TItem> source)
        {
	        return source.ApplyFiltersWithGroupingToQueryable(SortingOptions, GroupingOptions, PageableOptions);
        }

        private IQueryable<TItem> ApplyFiltersToQueryable(IQueryable<TItem> queryable)
        {
	        return queryable.ApplySortingToQueryable(SortingOptions)
		        .ApplyPagingToQueryable(PageableOptions);
        }
    }
}
