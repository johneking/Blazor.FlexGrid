﻿using Blazor.FlexGrid.Components.Configuration;
using Blazor.FlexGrid.Components.Configuration.MetaData;
using Blazor.FlexGrid.Components.Configuration.ValueFormatters;
using Blazor.FlexGrid.Components.Filters;
using Blazor.FlexGrid.Components.Renderers.EditInputs;
using Blazor.FlexGrid.DataAdapters;
using Blazor.FlexGrid.DataSet;
using Blazor.FlexGrid.DataSet.Options;
using Blazor.FlexGrid.Permission;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blazor.FlexGrid.Components.Renderers
{
    public class GridRendererContext : IActualItemContext<object>
    {
        private readonly string _firstColumnName;
        private readonly string _lastColumnName;
        private readonly IEntityType _gridEntityConfiguration;
        private readonly IReadOnlyDictionary<string, IValueFormatter> _valueFormatters;
        private readonly IReadOnlyDictionary<string, IRenderFragmentAdapter> _columnRendererFragments;
        private readonly IReadOnlyDictionary<string, Func<EditColumnContext, IRenderFragmentAdapter>> _columnEditRendererBuilders;

        public string ActualColumnName { get; set; } = string.Empty;

        public bool ActualColumnPropertyCanBeEdited { get; set; }

        public bool IsFirstColumn => ActualColumnName.Equals(_firstColumnName);

        public bool IsLastColumn => ActualColumnName.Equals(_lastColumnName);

        public int NumberOfColumns { get; }

        public bool IsActualItemEdited => TableDataSet.IsItemEdited(ActualItem);

        public bool SortingByActualColumnName => TableDataSet.SortingOptions.SortExpression.Equals(ActualColumnName);

        public object ActualItem { get; set; }

        public IGridViewAnnotations GridConfiguration { get; }

        public IReadOnlyCollection<PropertyInfo> GridItemProperties { get; }

        public IReadOnlyCollection<PropertyInfo> GridItemCollectionProperties { get; }

        public ITableDataSet TableDataSet { get; }

        public IGridViewColumnAnnotations ActualColumnConfiguration => _gridEntityConfiguration.FindColumnConfiguration(ActualColumnName);

        public GridCssClasses CssClasses { get; }

        public ITypePropertyAccessor PropertyValueAccessor { get; }

        public IRendererTreeBuilder RendererTreeBuilder { get; }

        public FlexGridContext FlexGridContext { get; }

        public Action RequestRerenderNotification => FlexGridContext.RequestRerenderTableRowsNotification;

        public GridRendererContext(
            ImmutableGridRendererContext immutableGridRendererContext,
            IRendererTreeBuilder rendererTreeBuilder,
            ITableDataSet tableDataSet,
            FlexGridContext flexGridContext)
        {
            if (immutableGridRendererContext is null)
            {
                throw new ArgumentNullException(nameof(immutableGridRendererContext));
            }

            RendererTreeBuilder = rendererTreeBuilder ?? throw new ArgumentNullException(nameof(RendererTreeBuilder));
            TableDataSet = tableDataSet ?? throw new ArgumentNullException(nameof(tableDataSet));
            FlexGridContext = flexGridContext ?? throw new ArgumentNullException(nameof(flexGridContext));

            GridConfiguration = immutableGridRendererContext.GridConfiguration;
            GridItemProperties = immutableGridRendererContext.GridItemProperties;
            GridItemCollectionProperties = immutableGridRendererContext.GridEntityConfiguration.ClrTypeCollectionProperties;
            CssClasses = immutableGridRendererContext.CssClasses;
            PropertyValueAccessor = immutableGridRendererContext.GetPropertyValueAccessor;

            _gridEntityConfiguration = immutableGridRendererContext.GridEntityConfiguration;
            _valueFormatters = immutableGridRendererContext.ValueFormatters;
            _columnRendererFragments = immutableGridRendererContext.ColumnRendererFragments;
            _columnEditRendererBuilders = immutableGridRendererContext.ColumnEditRendererBuilders;
            _firstColumnName = GridItemProperties.First().Name;
            _lastColumnName = GridItemProperties.Last().Name;
            NumberOfColumns = GridItemProperties.Count +
                (immutableGridRendererContext.InlineEditItemIsAllowed() || immutableGridRendererContext.CreateItemIsAllowed() ? 1 : 0) +
                (GridConfiguration.IsMasterTable ? 1 : 0);
        }

        public void OpenElement(string elementName)
            => RendererTreeBuilder.OpenElement(elementName);

        public void CloseElement()
            => RendererTreeBuilder.CloseElement();

        public void AddCssClass(string className)
            => RendererTreeBuilder.AddAttribute(HtmlAttributes.Class, className);

        public void AddHeaderStyle(string style)
            => RendererTreeBuilder.AddAttribute(HtmlAttributes.Style, style);

        public void AddOnClickEvent(EventCallback<MouseEventArgs> onClickBindMethod)
            => RendererTreeBuilder.AddAttribute(HtmlJsEvents.OnClick, onClickBindMethod);

        public void AddOnChangeEvent(Func<MulticastDelegate> onClickBindMethod)
            => RendererTreeBuilder.AddAttribute(HtmlJsEvents.OnChange, onClickBindMethod());

        public void AddContent(string content)
            => RendererTreeBuilder.AddContent(content);

        public void AddMarkupContent(string content)
            => RendererTreeBuilder.AddContent(new MarkupString(content));

        public void AddActualColumnValue(PermissionContext permissionContext)
        {
            if (!permissionContext.HasCurrentUserReadPermission(ActualColumnName))
            {
                RendererTreeBuilder.AddContent("*****");
                return;
            }

            if (_columnRendererFragments.TryGetValue(ActualColumnName, out var rendererFragmentAdapter))
            {
                var fragment = rendererFragmentAdapter.GetColumnFragment(ActualItem);
                RendererTreeBuilder.AddContent(fragment);
                return;
            }

            var valueFormatter = _valueFormatters[ActualColumnName];
            var inputForColumnValueFormatter = valueFormatter.FormatterType == ValueFormatterType.SingleProperty
                ? PropertyValueAccessor.GetValue(ActualItem, ActualColumnName)
                : ActualItem;

            RendererTreeBuilder.AddContent(new MarkupString(
               valueFormatter.FormatValue(inputForColumnValueFormatter))
            );
        }

        public void AddEditField(EditInputRendererTree editInputRendererTree, PermissionContext permissionContext)
        {
            if (!ActualColumnPropertyCanBeEdited ||
                !permissionContext.HasCurrentUserWritePermission(ActualColumnName))
            {
                AddActualColumnValue(permissionContext);

                return;
            }

            if (_columnEditRendererBuilders.TryGetValue(ActualColumnName, out var builder))
            {
                var editColumnContext = new EditColumnContext(ActualColumnName, TableDataSet.EditItemProperty);
                var rendererFragmentAdapter = builder.Invoke(editColumnContext);
                var fragment = rendererFragmentAdapter.GetColumnFragment(ActualItem);
                RendererTreeBuilder.AddContent(fragment);

                return;
            }

            editInputRendererTree.BuildInputRendererTree(
                RendererTreeBuilder,
                this,
                TableDataSet.EditItemProperty,
                ActualColumnName);

        }

        public void AddDisabled(bool disabled)
            => RendererTreeBuilder.AddAttribute(HtmlAttributes.Disabled, disabled);

        public void AddColspan()
        {
            RendererTreeBuilder.AddAttribute(HtmlAttributes.Colspan, GridItemProperties.Count + 1);
            RendererTreeBuilder.AddContent(string.Empty);
        }

        public void AddEmptyItemsMessage()
            => AddMarkupContent($"\t {GridConfiguration.EmptyItemsMessage}");

        public void OpenElement(string elementName, string className, string style = null)
        {
            OpenElement(elementName);
            AddCssClass(className);
            if (!string.IsNullOrEmpty(style))
            {
                AddHeaderStyle(style);
            }
        }

        public void AddAttribute(string name, object value)
            => RendererTreeBuilder.AddAttribute(name, value);

        public void AddAttribute(string name, Action<ChangeEventArgs> value)
            => RendererTreeBuilder.AddAttribute(name, value);

        public void AddDetailGridViewComponent(ITableDataAdapter tableDataAdapter)
        {
            if (tableDataAdapter is null)
            {
                return;
            }

            var masterDetailRelationship = GridConfiguration.FindRelationshipConfiguration(tableDataAdapter.UnderlyingTypeOfItem);
            var pageSize = masterDetailRelationship.DetailGridViewPageSize(TableDataSet);

            RendererTreeBuilder.OpenComponent(typeof(GridViewGeneric<>).MakeGenericType(tableDataAdapter.UnderlyingTypeOfItem));
            RendererTreeBuilder.AddAttribute("DataAdapter", tableDataAdapter);
            RendererTreeBuilder.AddAttribute(nameof(ITableDataSet.PageableOptions.PageSize), pageSize);

            RendererTreeBuilder.AddAttribute(
                nameof(ILazyTableDataSet.LazyLoadingOptions),
                new LazyLoadingOptions
                {
                    DataUri = masterDetailRelationship.DetailGridLazyLoadingUrl(),
                    PutDataUri = masterDetailRelationship.DetailGridUpdateUrl(),
                    DeleteUri = masterDetailRelationship.DetailGridDeleteUrl()
                });

            AddEventAttributes();
            RendererTreeBuilder.CloseComponent();
        }

        public void AddGridViewComponent(ITableDataAdapter tableDataAdapter)
        {
            RendererTreeBuilder.OpenComponent(typeof(GridViewGroup<>).MakeGenericType(tableDataAdapter.UnderlyingTypeOfItem));
            RendererTreeBuilder.AddAttribute("DataAdapter", tableDataAdapter);
            RendererTreeBuilder.AddAttribute(nameof(ITableDataSet.PageableOptions.PageSize), GridConfiguration.GroupingOptions.GroupPageSize);
            RendererTreeBuilder.CloseComponent();
        }

        public void AddFilterComponent(PropertyInfo property)
        {
            RendererTreeBuilder
                .OpenComponent(typeof(ColumnFilter<>).MakeGenericType(property.PropertyType))
                .AddAttribute("ColumnName", property.Name)
                .CloseComponent();
        }

        public object GetActualItemColumnValue(string columnName)
            => PropertyValueAccessor.GetValue(ActualItem, columnName);

        public void SetActualItemColumnValue(string columnName, object value)
            => PropertyValueAccessor.SetValue(ActualItem, columnName, value);

        public string GetColumnCaption(string columnName)
            => _gridEntityConfiguration.FindColumnConfiguration(columnName)?.Caption;

        private void AddEventAttributes()
        {
            if (TableDataSet.GridViewEvents.SaveOperationFinished != null)
            {
                RendererTreeBuilder.AddAttribute(
                    nameof(ITableDataSet.GridViewEvents.SaveOperationFinished),
                    TableDataSet.GridViewEvents.SaveOperationFinished);
            }

            if (TableDataSet.GridViewEvents.NewItemCreated != null)
            {
                RendererTreeBuilder.AddAttribute(
                    nameof(ITableDataSet.GridViewEvents.NewItemCreated),
                    TableDataSet.GridViewEvents.NewItemCreated);
            }

            if (TableDataSet.GridViewEvents.OnItemClicked != null)
            {
                RendererTreeBuilder.AddAttribute(
                    nameof(ITableDataSet.GridViewEvents.OnItemClicked),
                    TableDataSet.GridViewEvents.OnItemClicked);
            }

            if (TableDataSet.GridViewEvents.DeleteOperationFinished != null)
            {
                RendererTreeBuilder.AddAttribute(
                    nameof(ITableDataSet.GridViewEvents.DeleteOperationFinished),
                    TableDataSet.GridViewEvents.DeleteOperationFinished);
            }
        }
    }
}
