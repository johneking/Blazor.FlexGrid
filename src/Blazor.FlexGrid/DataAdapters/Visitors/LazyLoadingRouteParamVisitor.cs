using Blazor.FlexGrid.Components.Configuration.MetaData;
using Blazor.FlexGrid.Components.Configuration.ValueFormatters;
using Blazor.FlexGrid.DataSet.Options;
using System;

namespace Blazor.FlexGrid.DataAdapters.Visitors
{
    public class LazyLoadingRouteParamVisitor : IDataTableAdapterVisitor
    {
        private readonly IMasterDetailRelationship _masterDetailRelationship;
        private readonly IMasterDetailRowArguments _masterDetailRowArguments;
        private readonly ITypePropertyAccessorCache _propertyValueAccessorCache;

        public LazyLoadingRouteParamVisitor(
            IMasterDetailRelationship masterDetailRelationship,
            IMasterDetailRowArguments masterDetailRowArguments,
            ITypePropertyAccessorCache propertyValueAccessorCache
            )
        {
            _masterDetailRelationship = masterDetailRelationship ?? throw new ArgumentNullException(nameof(masterDetailRelationship));
            _masterDetailRowArguments = masterDetailRowArguments ?? throw new ArgumentNullException(nameof(masterDetailRowArguments));
            _propertyValueAccessorCache = propertyValueAccessorCache ?? throw new ArgumentNullException(nameof(propertyValueAccessorCache));
        }

        public void Visit(ITableDataAdapter tableDataAdapter)
        {
            if (tableDataAdapter is ILazyLoadedTableDataAdapter lazyLoadedTableDataAdapter)
            {
                var selectedItemType = _masterDetailRowArguments.SelectedItem.GetType();

                var constantValue = _propertyValueAccessorCache
                    .GetPropertyAccesor(selectedItemType)
                    .GetValue(_masterDetailRowArguments.SelectedItem, _masterDetailRelationship.MasterDetailConnection.MasterPropertyName);

                lazyLoadedTableDataAdapter.AddRequestParamsAction = reqParams => reqParams
                    .Add(_masterDetailRelationship.MasterDetailConnection.ForeignPropertyName, constantValue.ToString());
            }
        }
    }
}
