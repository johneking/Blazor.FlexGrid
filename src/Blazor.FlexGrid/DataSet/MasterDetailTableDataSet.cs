using Blazor.FlexGrid.Components.Configuration;
using Blazor.FlexGrid.Components.Configuration.MetaData;
using Blazor.FlexGrid.Components.Configuration.ValueFormatters;
using Blazor.FlexGrid.Components.Events;
using Blazor.FlexGrid.DataAdapters;
using Blazor.FlexGrid.DataSet.Options;
using Blazor.FlexGrid.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.FlexGrid.DataSet
{
    internal class MasterDetailTableDataSet<TItem> : IMasterTableDataSet, IBaseTableDataSet<TItem> where TItem : class
    {
        private readonly IEntityType _gridConfiguration;
        private readonly Dictionary<object, ITableDataAdapter> _selectedDataAdapters;
        private readonly ITableDataSet _tableDataSet;
        private readonly ITableDataAdapterProvider _tableDataAdapterProvider;
        private readonly HashSet<ITableDataAdapter> _tableDataAdapters;

        public IEnumerable<ITableDataAdapter> DetailDataAdapters => _tableDataAdapters;

        public IPagingOptions PageableOptions => _tableDataSet.PageableOptions;

        public ISortingOptions SortingOptions => _tableDataSet.SortingOptions;

        public IRowEditOptions RowEditOptions => _tableDataSet.RowEditOptions;

        public GridViewEvents GridViewEvents => _tableDataSet.GridViewEvents;

        public bool FilterIsApplied => _tableDataSet.FilterIsApplied;

        public IList<TItem> Items => _tableDataSet.Items as List<TItem>;

        IList IBaseTableDataSet.Items => Items is List<TItem> list ? list : Items.ToList();

        public IGroupingOptions GroupingOptions => _tableDataSet.GroupingOptions;

        public IList<GroupItem> GroupedItems => _tableDataSet.GroupedItems;

        public MasterDetailTableDataSet(
            ITableDataSet tableDataSet,
            IGridConfigurationProvider gridConfigurationProvider,
            ITableDataAdapterProvider tableDataAdapterProvider)
        {
            _tableDataSet = tableDataSet ?? throw new ArgumentNullException(nameof(tableDataSet));
            _tableDataAdapterProvider = tableDataAdapterProvider ?? throw new ArgumentNullException(nameof(tableDataAdapterProvider));
            _tableDataAdapters = new HashSet<ITableDataAdapter>();
            _selectedDataAdapters = new Dictionary<object, ITableDataAdapter>();
            _gridConfiguration = gridConfigurationProvider?.FindGridEntityConfigurationByType(typeof(TItem)) ?? throw new ArgumentNullException(nameof(gridConfigurationProvider));
        }

        public void AttachDetailDataSetAdapter(ITableDataAdapter tableDataAdapter)
        {
            if (_tableDataAdapters.Contains(tableDataAdapter))
            {
                return;
            }

            _tableDataAdapters.Add(tableDataAdapter);
        }

        public ITableDataAdapter GetSelectedDataAdapter(object selectedItem)
            => _selectedDataAdapters[selectedItem];

        public void SelectDataAdapter(IMasterDetailRowArguments masterDetailRowArguments)
        {
            if (masterDetailRowArguments is null)
            {
                throw new ArgumentNullException(nameof(masterDetailRowArguments));
            }

            _selectedDataAdapters[masterDetailRowArguments.SelectedItem] = _tableDataAdapterProvider.ConvertToDetailTableDataAdapter(
                masterDetailRowArguments.DataAdapter,
                masterDetailRowArguments.SelectedItem);
        }

        public Task GoToPage(int index)
            => _tableDataSet.GoToPage(index);

        public Task SetSortExpression(string expression)
            => _tableDataSet.SetSortExpression(expression);

        public void ToggleRowItem(object item)
        {
            _tableDataSet.ToggleRowItem(item);
            if (_selectedDataAdapters.ContainsKey(item))
            {
                return;
            }

            var tableDataAdapter = default(ITableDataAdapter);

            if (_tableDataAdapters.Any())
            {
                tableDataAdapter = _tableDataAdapterProvider.ConvertToDetailTableDataAdapter(
                    _tableDataAdapters.First(), item);
            }
            else
            {
                tableDataAdapter = _tableDataAdapterProvider.CreateCollectionTableDataAdapter(
                    item, _gridConfiguration.ClrTypeCollectionProperties.First());
            }

            _selectedDataAdapters.Add(item, tableDataAdapter);
        }

        public bool ItemIsSelected(object item)
            => _tableDataSet.ItemIsSelected(item);

        public void StartEditItem(object item)
            => _tableDataSet.StartEditItem(item);

        public void CancelEdit()
            => _tableDataSet.CancelEdit();

        public void EditItemProperty(string propertyName, object propertyValue)
            => _tableDataSet.EditItemProperty(propertyName, propertyValue);

        public Task<bool> SaveItem(ITypePropertyAccessor propertyValueAccessor)
            => _tableDataSet.SaveItem(propertyValueAccessor);

        public Task<bool> DeleteItem(object item)
            => _tableDataSet.DeleteItem(item);

        public Task ApplyFilters(IReadOnlyCollection<IFilterDefinition> filters)
            => _tableDataSet.ApplyFilters(filters);

        public void ToggleGroupRow(object groupItemKey)
            => _tableDataSet.ToggleGroupRow(groupItemKey);
    }
}
