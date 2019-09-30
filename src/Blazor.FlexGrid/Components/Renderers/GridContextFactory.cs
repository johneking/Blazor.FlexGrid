using Blazor.FlexGrid.Components.Configuration;
using Blazor.FlexGrid.Components.Configuration.ValueFormatters;
using Blazor.FlexGrid.DataSet;
using Blazor.FlexGrid.Permission;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.FlexGrid.Components.Renderers
{
    public class GridContextFactory
    {
        private readonly Dictionary<Type, ImmutableGridRendererContext> _immutableRendererContextCache;
        private readonly ITypePropertyAccessorCache _propertyValueAccessorCache;
        private readonly ICurrentUserPermission _currentUserPermission;
        private readonly ILogger<GridContextFactory> _logger;

        private IGridConfigurationProvider GridConfigurationProvider { get; }

        public GridContextFactory(
            IGridConfigurationProvider gridConfigurationProvider,
            ITypePropertyAccessorCache propertyValueAccessorCache,
            ICurrentUserPermission currentUserPermission,
            ILogger<GridContextFactory> logger)
        {
            GridConfigurationProvider = gridConfigurationProvider ?? throw new ArgumentNullException(nameof(gridConfigurationProvider));
            _propertyValueAccessorCache = propertyValueAccessorCache ?? throw new ArgumentNullException(nameof(propertyValueAccessorCache));
            _currentUserPermission = currentUserPermission ?? throw new ArgumentNullException(nameof(currentUserPermission));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _immutableRendererContextCache = new Dictionary<Type, ImmutableGridRendererContext>();
        }

        public (ImmutableGridRendererContext ImutableRendererContext, PermissionContext PermissionContext) CreateContexts(ITableDataSet tableDataSet)
        {
            var immutableRendererContext = GetImmutableGridRendererContext(tableDataSet.UnderlyingTypeOfItem());

            return (immutableRendererContext, immutableRendererContext.PermissionContext);
        }

        private ImmutableGridRendererContext GetImmutableGridRendererContext(Type dataSetItemType)
        {
            if (_immutableRendererContextCache.TryGetValue(dataSetItemType, out var immutableGridRendererContext))
            {
                return immutableGridRendererContext;
            }

            var gridConfiguration = GridConfigurationProvider.FindGridEntityConfigurationByType(dataSetItemType);
            _propertyValueAccessorCache.AddPropertyAccessor(dataSetItemType, new TypeWrapper(dataSetItemType, _logger));

            immutableGridRendererContext = new ImmutableGridRendererContext(
                    gridConfiguration,
                    _propertyValueAccessorCache.GetPropertyAccesor(dataSetItemType),
                    _currentUserPermission);

            immutableGridRendererContext.InitializeGridProperties(dataSetItemType.GetProperties().ToList());
            _immutableRendererContextCache.Add(dataSetItemType, immutableGridRendererContext);

            return immutableGridRendererContext;
        }
    }
}
