﻿using Blazor.FlexGrid.Components.Events;
using Blazor.FlexGrid.DataSet;
using Blazor.FlexGrid.Permission;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Blazor.FlexGrid.Components.Renderers
{
    public class GridRowRenderer : GridCompositeRenderer
    {
        public override bool CanRender(GridRendererContext rendererContext)
            => !(rendererContext.ActualItem is GroupItem
            || rendererContext.GetType().BaseType == typeof(GroupItem));

        protected override void BuildRenderTreeInternal(GridRendererContext rendererContext, PermissionContext permissionContext)
        {
            rendererContext.OpenElement(HtmlTagNames.TableRow, rendererContext.CssClasses.TableRow);

            var localActualItem = rendererContext.ActualItem;
            rendererContext.AddOnClickEvent(
                EventCallback.Factory.Create(this, (MouseEventArgs e) =>
                {
                    rendererContext.TableDataSet
                         .GridViewEvents
                         .OnItemClicked?.Invoke(new ItemClickedArgs { Item = localActualItem });

                }));

            foreach (var property in rendererContext.GridItemProperties)
            {
                rendererContext.ActualColumnName = property.Name;
                rendererContext.ActualColumnPropertyCanBeEdited = property.CanWrite;

                GridPartRenderers.ForEach(renderer => renderer.BuildRendererTree(rendererContext, permissionContext));
            }

            rendererContext.CloseElement();

            GridPartRenderersAfter.ForEach(renderer => renderer.BuildRendererTree(rendererContext, permissionContext));
        }
    }
}
