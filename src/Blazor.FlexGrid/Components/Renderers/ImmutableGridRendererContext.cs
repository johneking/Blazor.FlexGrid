using Blazor.FlexGrid.Components.Configuration;
using Blazor.FlexGrid.Components.Configuration.MetaData;
using Blazor.FlexGrid.Components.Configuration.ValueFormatters;
using Blazor.FlexGrid.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blazor.FlexGrid.Components.Renderers
{
    public class ImmutableGridRendererContext
    {
        private readonly Dictionary<string, IValueFormatter> _valueFormatters;
        private readonly Dictionary<string, IRenderFragmentAdapter> _columnRendererFragments;
        private readonly Dictionary<string, Func<EditColumnContext, IRenderFragmentAdapter>> _columnEditRendererBuilders;

        public IEntityType GridEntityConfiguration { get; }

        public IGridViewAnnotations GridConfiguration { get; }

        public IReadOnlyCollection<PropertyInfo> GridItemProperties { get; private set; }

        public ITypePropertyAccessor GetPropertyValueAccessor { get; }

        public IReadOnlyDictionary<string, IValueFormatter> ValueFormatters => _valueFormatters;

        public IReadOnlyDictionary<string, IRenderFragmentAdapter> ColumnRendererFragments => _columnRendererFragments;

        public IReadOnlyDictionary<string, Func<EditColumnContext, IRenderFragmentAdapter>> ColumnEditRendererBuilders => _columnEditRendererBuilders;

        public GridCssClasses CssClasses { get; }

        public PermissionContext PermissionContext { get; }

        public ImmutableGridRendererContext(
            IEntityType gridEntityConfiguration,
            ITypePropertyAccessor propertyValueAccessor,
            ICurrentUserPermission currentUserPermission)
        {
            _valueFormatters = new Dictionary<string, IValueFormatter>();
            _columnRendererFragments = new Dictionary<string, IRenderFragmentAdapter>();
            _columnEditRendererBuilders = new Dictionary<string, Func<EditColumnContext, IRenderFragmentAdapter>>();

            GridEntityConfiguration = gridEntityConfiguration ?? throw new ArgumentNullException(nameof(gridEntityConfiguration));
            GetPropertyValueAccessor = propertyValueAccessor ?? throw new ArgumentNullException(nameof(propertyValueAccessor));

            PermissionContext = new PermissionContext(currentUserPermission, gridEntityConfiguration);
            GridConfiguration = new GridAnnotations(gridEntityConfiguration);
            CssClasses = GridConfiguration.CssClasses;
        }

        public void InitializeGridProperties(List<PropertyInfo> itemProperties)
        {
	        if (itemProperties is null)
            {
                throw new ArgumentNullException(nameof(itemProperties));
            }

            var collectionProperties = GridEntityConfiguration.ClrTypeCollectionProperties;
            var propertiesListWithOrder = new List<(int Order, PropertyInfo Prop)>();

            foreach (var property in itemProperties)
            {
                var columnConfig = GridEntityConfiguration.FindColumnConfiguration(property.Name);

                if (columnConfig == null && GridConfiguration.OnlyShowExplicitProperties)
                    continue;

                PermissionContext.ResolveColumnPermission(columnConfig, property.Name);

                if (columnConfig?.ValueFormatter == null && collectionProperties.Contains(property))
                {
                    continue;
                }

                var columnVisibility = columnConfig?.IsVisible;
                if (columnVisibility.HasValue && !columnVisibility.Value)
                {
                    continue;
                }

                var columnOrder = columnConfig == null ? GridColumnAnnotations.DefaultOrder : columnConfig.Order;
                var columnValueFormatter = columnConfig?.ValueFormatter ?? new DefaultValueFormatter();

                propertiesListWithOrder.Add((Order: columnOrder, Prop: property));
                _valueFormatters.Add(property.Name, columnValueFormatter);

                if (columnConfig?.SpecialColumnValue != null)
                {
                    _columnRendererFragments.Add(property.Name, columnConfig.SpecialColumnValue);
                }

                if (columnConfig?.ColumnEditComponentBuilder != null)
                {
                    _columnEditRendererBuilders.Add(property.Name, columnConfig.ColumnEditComponentBuilder);
                }
            }

            GridItemProperties = propertiesListWithOrder.OrderBy(p => p.Order)
                .Select(p => p.Prop)
                .ToList();
        }
    }
}
