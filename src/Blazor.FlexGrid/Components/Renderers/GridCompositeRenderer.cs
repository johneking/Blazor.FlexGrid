using Blazor.FlexGrid.Permission;
using System.Collections.Generic;

namespace Blazor.FlexGrid.Components.Renderers
{
    public abstract class GridCompositeRenderer : IGridRendererTreeBuilder
    {
        protected readonly List<IGridRendererTreeBuilder> GridPartRenderers;
        protected readonly List<IGridRendererTreeBuilder> GridPartRenderersBefore;
        protected readonly List<IGridRendererTreeBuilder> GridPartRenderersAfter;

        protected GridCompositeRenderer()
        {
            GridPartRenderers = new List<IGridRendererTreeBuilder>();
            GridPartRenderersBefore = new List<IGridRendererTreeBuilder>();
            GridPartRenderersAfter = new List<IGridRendererTreeBuilder>();
        }

        public virtual IGridRendererTreeBuilder AddRenderer(IGridRendererTreeBuilder gridPartRenderer, RendererType rendererPosition = RendererType.InsideTag)
        {
            switch (rendererPosition)
            {
                case RendererType.AfterTag:
                    GridPartRenderersAfter.Add(gridPartRenderer);
                    break;
                case RendererType.BeforeTag:
                    GridPartRenderersBefore.Add(gridPartRenderer);
                    break;
                case RendererType.InsideTag:
                    GridPartRenderers.Add(gridPartRenderer);
                    break;
            }

            return this;
        }

        public void BuildRendererTree(GridRendererContext rendererContext, PermissionContext permissionContext)
        {
            if (!CanRender(rendererContext))
            {
                return;
            }

            BuildRenderTreeInternal(rendererContext, permissionContext);
        }

        public abstract bool CanRender(GridRendererContext rendererContext);

        protected abstract void BuildRenderTreeInternal(GridRendererContext rendererContext, PermissionContext permissionContext);
    }
}
