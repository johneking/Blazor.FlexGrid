using Blazor.FlexGrid.Components.Configuration;
using Blazor.FlexGrid.DataAdapters;
using System;

namespace Blazor.FlexGrid.DataSet
{
    public class MasterDetailTableDataSetFactory : IMasterDetailTableDataSetFactory
    {
        private readonly IGridConfigurationProvider _gridConfigurationProvider;
        private readonly ITableDataAdapterProvider _tableDataAdapterProvider;

        public MasterDetailTableDataSetFactory(IGridConfigurationProvider gridConfigurationProvider, ITableDataAdapterProvider tableDataAdapterProvider)
        {
            _gridConfigurationProvider = gridConfigurationProvider ?? throw new ArgumentNullException(nameof(gridConfigurationProvider));
            _tableDataAdapterProvider = tableDataAdapterProvider ?? throw new ArgumentNullException(nameof(tableDataAdapterProvider));
        }

        public ITableDataSet ConvertToMasterTableIfIsRequired(ITableDataSet tableDataSet)
        {
            if (tableDataSet is IMasterTableDataSet masterTableDataSet)
            {
                return masterTableDataSet;
            }

            var tableDataSetItemType = tableDataSet.UnderlyingTypeOfItem();
            var entityConfiguration = _gridConfigurationProvider.GetGridConfigurationByType(tableDataSetItemType);
            if (!entityConfiguration.IsMasterTable)
            {
                return tableDataSet;
            }

            var masterDetailTableDataSetType = typeof(MasterDetailTableDataSet<>).MakeGenericType(tableDataSetItemType);
            var masterDetailTableDataSet = Activator.CreateInstance(masterDetailTableDataSetType,
                new object[] { tableDataSet, _gridConfigurationProvider, _tableDataAdapterProvider }) as IMasterTableDataSet;

            return masterDetailTableDataSet;
        }
    }
}
