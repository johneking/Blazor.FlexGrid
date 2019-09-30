using Blazor.FlexGrid.Permission;
using Microsoft.Extensions.Logging;
using System;

namespace Blazor.FlexGrid.Components.Renderers
{
    /// <summary>
    /// The 'Composite Renderer' 
    /// </summary>
    public class GridRenderer : GridCompositeRenderer
    {
        private readonly ILogger<GridRenderer> _logger;

        public GridRenderer(ILogger<GridRenderer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override bool CanRender(GridRendererContext rendererContext)
            => true;

        protected override void BuildRenderTreeInternal(GridRendererContext rendererContext, PermissionContext permissionContext)
        {
            using (new MeasurableScope(sw => _logger.LogInformation($"Grid rendering duration {sw.ElapsedMilliseconds}ms")))
            {
                try
                {
                    GridPartRenderersBefore.ForEach(renderer => renderer.BuildRendererTree(rendererContext, permissionContext));

                    rendererContext.OpenElement(HtmlTagNames.Div, "table-wrapper");
                    rendererContext.OpenElement(HtmlTagNames.Table, rendererContext.CssClasses.Table);

                    GridPartRenderers.ForEach(renderer => renderer.BuildRendererTree(rendererContext, permissionContext));

                    rendererContext.CloseElement(); // Close table

                    GridPartRenderersAfter.ForEach(renderer => renderer.BuildRendererTree(rendererContext, permissionContext));

                    rendererContext.CloseElement(); // Close table wrapper                
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error raised during rendering GridView component. Ex: {ex}");
                }
            }
        }
    }
}
