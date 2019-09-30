using Blazor.FlexGrid.Components.Configuration;
using Blazor.FlexGrid.DataSet;
using Blazor.FlexGrid.DataSet.Options;
using System;

namespace Blazor.FlexGrid.DataAdapters
{
    internal class DetailTableDataAdapter<TItem> : BaseTableDataAdapter where TItem : class
    {
        private readonly IGridConfigurationProvider _gridConfigurationProvider;
        private readonly IDetailDataAdapterVisitors _detailDataAdapterVisitors;
        private readonly IMasterDetailRowArguments _masterDetailRowArguments;

        public override Type UnderlyingTypeOfItem => typeof(TItem);

        public DetailTableDataAdapter(
            IGridConfigurationProvider gridConfigurationProvider,
            IDetailDataAdapterVisitors detailDataAdapterVisitors,
            IMasterDetailRowArguments masterDetailRowArguments)
        {
            _gridConfigurationProvider = gridConfigurationProvider ?? throw new ArgumentNullException(nameof(gridConfigurationProvider));
            _detailDataAdapterVisitors = detailDataAdapterVisitors ?? throw new ArgumentNullException(nameof(detailDataAdapterVisitors));
            _masterDetailRowArguments = masterDetailRowArguments ?? throw new ArgumentNullException(nameof(masterDetailRowArguments));
        }

        public override ITableDataSet GetTableDataSet(Action<TableDataSetOptions> configureDataSet)
        {
            var masterDetailRelation = _gridConfigurationProvider
                .GetGridConfigurationByType(_masterDetailRowArguments.SelectedItem.GetType())
                .FindRelationshipConfiguration(UnderlyingTypeOfItem);

            if (masterDetailRelation.IsOwnCollection)
            {
                return _masterDetailRowArguments.DataAdapter.GetTableDataSet(configureDataSet);
            }

            var clonedDataAdapter = _masterDetailRowArguments.DataAdapter.Clone() as ITableDataAdapter;
            foreach (var visitor in _detailDataAdapterVisitors.GetVisitors(_masterDetailRowArguments))
            {
                clonedDataAdapter.Accept(visitor);
            }

            return clonedDataAdapter.GetTableDataSet(configureDataSet);
        }

        public override object Clone()
            => new DetailTableDataAdapter<TItem>(_gridConfigurationProvider, _detailDataAdapterVisitors, _masterDetailRowArguments);
    }
}
