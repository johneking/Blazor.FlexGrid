using Blazor.FlexGrid.Components.Configuration;
using Blazor.FlexGrid.DataSet;
using Blazor.FlexGrid.DataSet.Options;
using System;
using System.Collections.Generic;

namespace Blazor.FlexGrid.DataAdapters
{
    public sealed class MasterTableDataAdapter<TItem> : BaseTableDataAdapter, IMasterTableDataAdapter where TItem : class
    {
        private readonly ITableDataAdapter _mainTableDataAdapter;
        private readonly IGridConfigurationProvider _gridConfigurationProvider;
        private readonly ITableDataAdapterProvider _tableDataAdapterProvider;
        private readonly List<ITableDataAdapter> _detailTableDataAdapters;

        public override Type UnderlyingTypeOfItem => typeof(TItem);

        public IReadOnlyCollection<ITableDataAdapter> DetailTableDataAdapters => _detailTableDataAdapters;

        public MasterTableDataAdapter(
            CollectionTableDataAdapter<TItem> mainTableDataAdapter,
            IGridConfigurationProvider gridConfigurationProvider,
            ITableDataAdapterProvider tableDataAdapterProvider)
            : this(gridConfigurationProvider, tableDataAdapterProvider, mainTableDataAdapter)
        {
        }

        public MasterTableDataAdapter(
            LazyLoadedTableDataAdapter<TItem> mainTableDataAdapter,
            IGridConfigurationProvider gridConfigurationProvider,
            ITableDataAdapterProvider tableDataAdapterProvider)
            : this(gridConfigurationProvider, tableDataAdapterProvider, mainTableDataAdapter)
        {
        }

        internal MasterTableDataAdapter(
            IGridConfigurationProvider gridConfigurationProvider,
            ITableDataAdapterProvider tableDataAdapterProvider,
            ITableDataAdapter mainTableDataAdapter)
        {
            _mainTableDataAdapter = mainTableDataAdapter ?? throw new ArgumentNullException(nameof(mainTableDataAdapter));
            _gridConfigurationProvider = gridConfigurationProvider ?? throw new ArgumentNullException(nameof(gridConfigurationProvider));
            _tableDataAdapterProvider = tableDataAdapterProvider ?? throw new ArgumentNullException(nameof(tableDataAdapterProvider));
            _detailTableDataAdapters = new List<ITableDataAdapter>();
        }

        public void AddDetailTableSet(ITableDataAdapter tableDataAdapter)
        {
            if (tableDataAdapter is null)
            {
                throw new ArgumentNullException(nameof(tableDataAdapter));
            }

            _detailTableDataAdapters.Add(tableDataAdapter);
        }

        public override ITableDataSet GetTableDataSet(Action<TableDataSetOptions> configureDataSet)
        {
            var mainTableDataSet = _mainTableDataAdapter.GetTableDataSet(configureDataSet);
            var masterTableDataSet = new MasterDetailTableDataSet<TItem>(mainTableDataSet, _gridConfigurationProvider, _tableDataAdapterProvider);

            _detailTableDataAdapters.ForEach(dt => masterTableDataSet.AttachDetailDataSetAdapter(dt));

            return masterTableDataSet;
        }

        public override object Clone()
        {
            var masterTableAdapter = new MasterTableDataAdapter<TItem>(_gridConfigurationProvider, _tableDataAdapterProvider, _mainTableDataAdapter);
            _detailTableDataAdapters.ForEach(adapter => masterTableAdapter.AddDetailTableSet(adapter));

            return masterTableAdapter;
        }
    }
}
