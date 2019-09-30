using Blazor.FlexGrid.Components.Configuration;
using Blazor.FlexGrid.Components.Configuration.ValueFormatters;
using Blazor.FlexGrid.DataSet.Options;
using System;
using System.Collections.Generic;

namespace Blazor.FlexGrid.DataAdapters.Visitors
{
    public class DetailDataAdapterVisitors : IDetailDataAdapterVisitors
    {
        private readonly ITypePropertyAccessorCache _propertyValueAccessorCache;
        private readonly IGridConfigurationProvider _gridConfigurationProvider;

        public DetailDataAdapterVisitors(ITypePropertyAccessorCache propertyValueAccessorCache, IGridConfigurationProvider gridConfigurationProvider)
        {
            _propertyValueAccessorCache = propertyValueAccessorCache ?? throw new ArgumentNullException(nameof(propertyValueAccessorCache));
            _gridConfigurationProvider = gridConfigurationProvider ?? throw new ArgumentNullException(nameof(gridConfigurationProvider));
        }

        public IEnumerable<IDataTableAdapterVisitor> GetVisitors(IMasterDetailRowArguments masterDetailRowArguments)
        {
            var selectedItemType = masterDetailRowArguments.SelectedItem.GetType();
            var detailAdapterItemType = masterDetailRowArguments.DataAdapter.UnderlyingTypeOfItem;
            var masterDetailConfiguration = _gridConfigurationProvider
                .GetGridConfigurationByType(selectedItemType)
                .FindRelationshipConfiguration(detailAdapterItemType);

            if (masterDetailRowArguments.DataAdapter is ILazyLoadedTableDataAdapter)
            {
                return new List<IDataTableAdapterVisitor>
               {
                   new LazyLoadingRouteParamVisitor(masterDetailConfiguration, masterDetailRowArguments, _propertyValueAccessorCache)
               };
            }

            var filterVisitorType = typeof(FilterVisitor<>).MakeGenericType(detailAdapterItemType);
            var filterVisitor = Activator.CreateInstance(filterVisitorType,
                new object[]
                {
                    masterDetailConfiguration,
                    masterDetailRowArguments,
                    _propertyValueAccessorCache
                }) as IDataTableAdapterVisitor;

            return new List<IDataTableAdapterVisitor>
            {
                filterVisitor
            };
        }
    }
}
