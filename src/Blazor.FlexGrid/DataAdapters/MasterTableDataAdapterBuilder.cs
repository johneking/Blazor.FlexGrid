using Blazor.FlexGrid.Components.Configuration;
using System;

namespace Blazor.FlexGrid.DataAdapters
{
    public class MasterTableDataAdapterBuilder<TItem> where TItem : class
    {
        private MasterTableDataAdapter<TItem> _masterTableDataAdapter;
        private readonly IGridConfigurationProvider _gridConfigurationProvider;
        private readonly ITableDataAdapterProvider _tableDataAdapterProvider;

        public MasterTableDataAdapterBuilder(IGridConfigurationProvider gridConfigurationProvider, ITableDataAdapterProvider tableDataAdapterProvider)
        {
            _gridConfigurationProvider = gridConfigurationProvider ?? throw new ArgumentNullException(nameof(gridConfigurationProvider));
            _tableDataAdapterProvider = tableDataAdapterProvider ?? throw new ArgumentNullException(nameof(tableDataAdapterProvider));
        }

        public MasterTableDataAdapterBuilder<TItem> MasterTableDataAdapter(CollectionTableDataAdapter<TItem> mainTableDataAdapter)
        {
            _masterTableDataAdapter = new MasterTableDataAdapter<TItem>(_gridConfigurationProvider, _tableDataAdapterProvider, mainTableDataAdapter);

            return this;
        }

        public MasterTableDataAdapterBuilder<TItem> MasterTableDataAdapter(LazyLoadedTableDataAdapter<TItem> mainTableDataAdapter)
        {
            _masterTableDataAdapter = new MasterTableDataAdapter<TItem>(_gridConfigurationProvider, _tableDataAdapterProvider, mainTableDataAdapter);

            return this;
        }

        public MasterTableDataAdapterBuilder<TItem> WithDetailTableDataAdapter(ITableDataAdapter tableDataAdapter)
        {
            _masterTableDataAdapter.AddDetailTableSet(tableDataAdapter);

            return this;
        }

        public MasterTableDataAdapter<TItem> Build()
        {
            if (_masterTableDataAdapter is null)
            {
                throw new InvalidOperationException($"Before build you must first call {nameof(MasterTableDataAdapter)}");
            }

            return _masterTableDataAdapter;
        }
    }
}
