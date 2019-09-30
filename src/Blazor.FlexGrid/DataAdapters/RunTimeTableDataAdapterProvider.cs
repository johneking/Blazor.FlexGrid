using Blazor.FlexGrid.Components.Configuration;
using Blazor.FlexGrid.Components.Configuration.ValueFormatters;
using Blazor.FlexGrid.DataSet;
using Blazor.FlexGrid.DataSet.Options;
using Blazor.FlexGrid.Features;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Blazor.FlexGrid.DataAdapters
{
    public class RunTimeTableDataAdapterProvider : ITableDataAdapterProvider
    {
        private readonly IGridConfigurationProvider _gridConfigurationProvider;
        private readonly IDetailDataAdapterVisitors _detailDataAdapterVisitors;
        private readonly ITypePropertyAccessorCache _propertyValueAccessorCache;

        public RunTimeTableDataAdapterProvider(
            IGridConfigurationProvider gridConfigurationProvider,
            ITypePropertyAccessorCache propertyValueAccessorCache,
            IDetailDataAdapterVisitors detailDataAdapterVisitors)
        {
            _gridConfigurationProvider = gridConfigurationProvider ?? throw new ArgumentNullException(nameof(gridConfigurationProvider));
            _detailDataAdapterVisitors = detailDataAdapterVisitors ?? throw new ArgumentNullException(nameof(detailDataAdapterVisitors));
            _propertyValueAccessorCache = propertyValueAccessorCache ?? throw new ArgumentNullException(nameof(propertyValueAccessorCache));
        }

        public ITableDataAdapter ConvertToDetailTableDataAdapter(ITableDataAdapter tableDataAdapter, object selectedItem)
        {
            var detailAdapterType = typeof(DetailTableDataAdapter<>).MakeGenericType(tableDataAdapter.UnderlyingTypeOfItem);

            return Activator.CreateInstance(detailAdapterType,
                new object[] { _gridConfigurationProvider, _detailDataAdapterVisitors, new MasterDetailRowArguments(tableDataAdapter, selectedItem) }) as ITableDataAdapter;
        }

        public ITableDataAdapter CreateCollectionTableDataAdapter(object selectedItem, PropertyInfo propertyInfo)
        {
            var propertyType = propertyInfo.PropertyType.GenericTypeArguments[0];
            var propertyValueGetter = _propertyValueAccessorCache.GetPropertyAccesor(selectedItem.GetType());
            var collectionValue = propertyValueGetter.GetValue(selectedItem, propertyInfo.Name);

            var dataAdapterType = typeof(CollectionTableDataAdapter<>).MakeGenericType(propertyType);
            var dataAdapter = Activator.CreateInstance(dataAdapterType,
                new object[] { collectionValue }) as ITableDataAdapter;

            return dataAdapter;
        }

        public ITableDataAdapter CreateCollectionTableDataAdapter(Type dataSetType, GroupItem group)
        {
            var subItemsListType = typeof(List<>).MakeGenericType(dataSetType);
            var subItemsList = Activator.CreateInstance(subItemsListType, new object[] { group });
            var dataAdapterType = typeof(CollectionTableDataAdapter<>).MakeGenericType(dataSetType);
            var dataAdapter = Activator.CreateInstance(dataAdapterType, new object[] { subItemsList }) as ITableDataAdapter;

            return dataAdapter;
        }

        public ITableDataAdapter CreateMasterTableDataAdapter(ITableDataAdapter mainTableDataAdapter, IMasterTableFeature masterTableFeature)
        {
            if (masterTableFeature == default(IMasterTableFeature))
            {
                return mainTableDataAdapter;
            }

            if (masterTableFeature.TableDataAdapter is IMasterTableDataAdapter masterTableDataAdapter)
            {
                var masterDataAdapterType = typeof(MasterTableDataAdapter<>).MakeGenericType(mainTableDataAdapter.UnderlyingTypeOfItem);
                var masterDataAdapter = Activator.CreateInstance(masterDataAdapterType,
                    new object[] { mainTableDataAdapter, _gridConfigurationProvider, this }) as IMasterTableDataAdapter;

                foreach (var detailAdapter in masterTableDataAdapter.DetailTableDataAdapters)
                {
                    masterDataAdapter.AddDetailTableSet(detailAdapter);
                }

                return masterDataAdapter as ITableDataAdapter;
            }

            return mainTableDataAdapter;
        }
    }
}
