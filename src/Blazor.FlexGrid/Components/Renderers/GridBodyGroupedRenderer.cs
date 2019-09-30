using Blazor.FlexGrid.DataAdapters;
using Blazor.FlexGrid.DataSet;
using Blazor.FlexGrid.Features;
using Blazor.FlexGrid.Permission;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using System;

namespace Blazor.FlexGrid.Components.Renderers
{
    public class GridBodyGroupedRenderer : GridCompositeRenderer
    {
        private readonly ITableDataAdapterProvider _tableDataAdapterProvider;
        private readonly ILogger<GridBodyGroupedRenderer> _logger;

        public GridBodyGroupedRenderer(ITableDataAdapterProvider tableDataAdapterProvider, ILogger<GridBodyGroupedRenderer> logger)
        {
            _tableDataAdapterProvider = tableDataAdapterProvider ?? throw new ArgumentNullException(nameof(tableDataAdapterProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override bool CanRender(GridRendererContext rendererContext)
            => rendererContext.TableDataSet.HasGroupedItems();

        protected override void BuildRenderTreeInternal(GridRendererContext rendererContext, PermissionContext permissionContext)
        {
            using (new MeasurableScope(sw => _logger.LogInformation($"Grid grouped body rendering duration {sw.ElapsedMilliseconds}ms")))
            {
                rendererContext.OpenElement(HtmlTagNames.TableBody, rendererContext.CssClasses.TableBody);
                foreach (var group in rendererContext.TableDataSet.GroupedItems)
                {
                    try
                    {
                        rendererContext.OpenElement(HtmlTagNames.TableRow, rendererContext.CssClasses.TableGroupRow);
                        rendererContext.OpenElement(HtmlTagNames.TableColumn, rendererContext.CssClasses.TableGroupRowCell);
                        rendererContext.AddAttribute(HtmlAttributes.Colspan, rendererContext.NumberOfColumns);
                        rendererContext.OpenElement(HtmlTagNames.Button, "pagination-button");
                        rendererContext.AddOnClickEvent(
                            EventCallback.Factory.Create(this, (MouseEventArgs e) =>
                            {
                                rendererContext.TableDataSet.ToggleGroupRow(group.Key);
                                rendererContext.RequestRerenderNotification?.Invoke();
                            })
                        );
                        rendererContext.OpenElement(HtmlTagNames.Span, "pagination-button-arrow");
                        rendererContext.OpenElement(HtmlTagNames.I, !group.IsCollapsed ? "fas fa-angle-down" : "fas fa-angle-right");
                        rendererContext.CloseElement();
                        rendererContext.CloseElement();
                        rendererContext.CloseElement();
                        rendererContext.AddMarkupContent($"\t<b>{rendererContext.TableDataSet.GroupingOptions.GroupedProperty.Name}:</b> {group.Key.ToString()}\t");
                        rendererContext.OpenElement(HtmlTagNames.I);
                        rendererContext.AddContent($"({group.Count})");
                        rendererContext.CloseElement();


                        if (!group.IsCollapsed)
                        {
                            var dataAdapter = _tableDataAdapterProvider.CreateCollectionTableDataAdapter(rendererContext.TableDataSet.UnderlyingTypeOfItem(), group);
                            var masterTableFeature = rendererContext.FlexGridContext.Features.Get<IMasterTableFeature>();
                            dataAdapter = _tableDataAdapterProvider.CreateMasterTableDataAdapter(dataAdapter, masterTableFeature);

                            rendererContext.AddGridViewComponent(dataAdapter);
                        }

                        rendererContext.CloseElement();
                        rendererContext.CloseElement();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error occured during rendering grouped grid view body. Ex: {ex}");

                        throw ex;
                    }
                }

                rendererContext.CloseElement();
            }
        }
    }
}

