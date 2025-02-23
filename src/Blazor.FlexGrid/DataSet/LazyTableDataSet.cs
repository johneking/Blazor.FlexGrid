﻿using Blazor.FlexGrid.Components.Configuration.ValueFormatters;
using Blazor.FlexGrid.Components.Events;
using Blazor.FlexGrid.DataSet.Http;
using Blazor.FlexGrid.DataSet.Options;
using Blazor.FlexGrid.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.FlexGrid.DataSet
{
    /// <summary>
    /// Collection of items which are fetched from server API
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class LazyTableDataSet<TItem> : ILazyTableDataSet, IBaseTableDataSet<TItem> where TItem : class
    {
        private readonly ILazyDataSetLoader<TItem> _lazyDataSetLoader;
        private readonly ILazyGroupableDataSetLoader<TItem> _lazyGroupableDataSetLoader;
        private readonly ILazyDataSetItemManipulator<TItem> _lazyDataSetItemSaver;
        private readonly HashSet<object> _selectedItems;
        private IReadOnlyCollection<IFilterDefinition> _filterDefinitions = new List<IFilterDefinition>();

        public IPagingOptions PageableOptions { get; set; } = new PageableOptions();

        public ISortingOptions SortingOptions { get; set; } = new SortingOptions();

        public ILazyLoadingOptions LazyLoadingOptions { get; set; } = new LazyLoadingOptions();

        public IRowEditOptions RowEditOptions { get; set; } = new RowEditOptions();

        public IGroupingOptions GroupingOptions { get; set; } = new GroupingOptions();

        public GridViewEvents GridViewEvents { get; set; } = new GridViewEvents();

        public bool FilterIsApplied => _filterDefinitions.Any();

        /// <summary>
        /// Gets or sets the items for the current page.
        /// </summary>
        public IList<TItem> Items { get; set; } = new List<TItem>();

        IList IBaseTableDataSet.Items => Items is List<TItem> list ? list : Items.ToList();

        public IList<GroupItem> GroupedItems { get; private set; } = new List<GroupItem>();

        public LazyTableDataSet(
            ILazyDataSetLoader<TItem> lazyDataSetLoader,
            ILazyGroupableDataSetLoader<TItem> lazyGroupableDataSetLoader,
            ILazyDataSetItemManipulator<TItem> lazyDataSetItemSaver)
        {
            _lazyDataSetLoader = lazyDataSetLoader ?? throw new ArgumentNullException(nameof(lazyDataSetLoader));
            _lazyGroupableDataSetLoader = lazyGroupableDataSetLoader ?? throw new ArgumentNullException(nameof(lazyGroupableDataSetLoader));
            _lazyDataSetItemSaver = lazyDataSetItemSaver ?? throw new ArgumentNullException(nameof(lazyDataSetItemSaver));
            _selectedItems = new HashSet<object>();
        }

        public async Task GoToPage(int index)
        {
            PageableOptions.CurrentPage = index;
            var requestOptions = new RequestOptions(LazyLoadingOptions, PageableOptions, SortingOptions, GroupingOptions);

            if (!GroupingOptions.IsGroupingActive)
            {
                var pagedDataResult = await _lazyDataSetLoader.GetTablePageData(requestOptions, _filterDefinitions);
                Items = pagedDataResult.Items;
                PageableOptions.TotalItemsCount = pagedDataResult.TotalCount;
            }
            else
            {
                var groupedDataResult = await _lazyGroupableDataSetLoader.GetGroupedTablePageData(requestOptions, _filterDefinitions);
                var newGroupedItems = groupedDataResult.Items.AsQueryable().OfType<GroupItem>().ToList();
                newGroupedItems.PreserveGroupCollapsing(GroupedItems);
                GroupedItems = newGroupedItems;
                PageableOptions.TotalItemsCount = groupedDataResult.TotalCount;
            }
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

        public async Task<bool> SaveItem(ITypePropertyAccessor propertyValueAccessor)
        {
            try
            {
                foreach (var newValue in RowEditOptions.UpdatedValues)
                {
                    propertyValueAccessor.SetValue(RowEditOptions.ItemInEditMode, newValue.Key, newValue.Value);
                }
            }
            catch (Exception)
            {
                GridViewEvents.SaveOperationFinished?.Invoke(new SaveResultArgs { ItemSucessfullySaved = false });
                RowEditOptions.ItemInEditMode = EmptyDataSetItem.Instance;

                return false;
            }

            var typedItem = (TItem)RowEditOptions.ItemInEditMode;
            var saveResult = await _lazyDataSetItemSaver.SaveItem(typedItem, LazyLoadingOptions);
            if (saveResult != null)
            {
                var itemIndex = Items.IndexOf(typedItem);
                if (itemIndex > -1)
                {
                    Items[itemIndex] = saveResult;
                }

                GridViewEvents.SaveOperationFinished?.Invoke(new SaveResultArgs { ItemSucessfullySaved = true, Item = saveResult });
            }

            RowEditOptions.ItemInEditMode = EmptyDataSetItem.Instance;

            return saveResult != null;
        }

        public void CancelEdit()
            => RowEditOptions.ItemInEditMode = EmptyDataSetItem.Instance;

        public async Task<bool> DeleteItem(object item)
        {
            var typedItem = (TItem)item;
            var removedItem = await _lazyDataSetItemSaver.DeleteItem(typedItem, LazyLoadingOptions);
            if (removedItem != null)
            {
                GridViewEvents.DeleteOperationFinished?.Invoke(new DeleteResultArgs { ItemSuccesfullyDeleted = true, Item = removedItem });
                await GoToPage(PageableOptions.CurrentPage);
            }
            else
            {
                GridViewEvents.DeleteOperationFinished?.Invoke(new DeleteResultArgs { ItemSuccesfullyDeleted = false, Item = item });
            }

            return removedItem != null;
        }

        public Task ApplyFilters(IReadOnlyCollection<IFilterDefinition> filters)
        {
            _filterDefinitions = filters;

            return GoToPage(0);
        }

        public void ToggleGroupRow(object groupItemKey)
            => GroupedItems.ToggleGroup(groupItemKey);
    }
}
