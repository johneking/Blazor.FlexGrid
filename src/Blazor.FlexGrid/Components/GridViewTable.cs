using Blazor.FlexGrid.Components.Renderers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazor.FlexGrid.Components
{
    internal class GridViewTable : ComponentBase
    {
        [Parameter] public RenderFragment<ImmutableGridRendererContext> ChildContent { get; set; }

        [Parameter] public ImmutableGridRendererContext ImmutableGridRendererContext { get; set; }

        [CascadingParameter] FlexGridContext CascadeFlexGridContext { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.OpenComponent<CascadingValue<ImmutableGridRendererContext>>(0);
            builder.AddAttribute(1, "Value", ImmutableGridRendererContext);
            builder.AddAttribute(2, BlazorRendererTreeBuilder.ChildContent, ChildContent?.Invoke(ImmutableGridRendererContext));
            builder.CloseComponent();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            CascadeFlexGridContext.SetRequestRendererNotification(StateHasChanged);
        }
    }
}
