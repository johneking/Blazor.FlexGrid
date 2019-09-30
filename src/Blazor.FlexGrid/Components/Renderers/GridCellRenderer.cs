using Blazor.FlexGrid.Components.Renderers.EditInputs;
using Blazor.FlexGrid.Permission;
using System;

namespace Blazor.FlexGrid.Components.Renderers
{
    public class GridCellRenderer : GridPartRenderer
    {
        private readonly EditInputRendererTree _editInputRendererTree;

        public GridCellRenderer(EditInputRendererTree editInputRendererTree)
        {
            _editInputRendererTree = editInputRendererTree ?? throw new ArgumentNullException(nameof(editInputRendererTree));
        }

        public override bool CanRender(GridRendererContext rendererContext)
            => true;

        protected override void BuildRendererTreeInternal(GridRendererContext rendererContext, PermissionContext permissionContext)
        {
            rendererContext.OpenElement(HtmlTagNames.TableColumn, rendererContext.CssClasses.TableCell);

            if (!rendererContext.IsActualItemEdited)
            {
                rendererContext.AddActualColumnValue(permissionContext);
                rendererContext.CloseElement();

                return;
            }

            rendererContext.AddEditField(_editInputRendererTree, permissionContext);
            rendererContext.CloseElement();
        }
    }
}
