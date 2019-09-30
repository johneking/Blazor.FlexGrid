using Blazor.FlexGrid.DataSet;
using Blazor.FlexGrid.Permission;
using System;

namespace Blazor.FlexGrid.Components.Renderers
{
    public class GridBodyRenderer : GridPartRenderer
    {
        private readonly IGridRendererTreeBuilder _simpleBodyRendererTreeBuilder;
        private readonly IGridRendererTreeBuilder _groupedBodyRendererTreeBuilder;

        public GridBodyRenderer(
            IGridRendererTreeBuilder simpleBodyRendererTreeBuilder,
            IGridRendererTreeBuilder groupedBodyRendererTreeBuilder)
        {
            _simpleBodyRendererTreeBuilder = simpleBodyRendererTreeBuilder ?? throw new ArgumentNullException(nameof(simpleBodyRendererTreeBuilder));
            _groupedBodyRendererTreeBuilder = groupedBodyRendererTreeBuilder ?? throw new ArgumentNullException(nameof(groupedBodyRendererTreeBuilder));
        }

        public override bool CanRender(GridRendererContext rendererContext)
            => rendererContext.TableDataSet.HasItems();

        protected override void BuildRendererTreeInternal(GridRendererContext rendererContext, PermissionContext permissionContext)
        {
            if (rendererContext.TableDataSet.GroupingOptions.IsGroupingActive)
            {
                _groupedBodyRendererTreeBuilder.BuildRendererTree(rendererContext, permissionContext);
            }
            else
            {
                _simpleBodyRendererTreeBuilder.BuildRendererTree(rendererContext, permissionContext);
            }
        }
    }
}
