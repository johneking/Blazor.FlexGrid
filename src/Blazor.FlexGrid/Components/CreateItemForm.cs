using Blazor.FlexGrid.Components.Configuration;
using Blazor.FlexGrid.Components.Configuration.ValueFormatters;
using Blazor.FlexGrid.Components.Renderers;
using Blazor.FlexGrid.Components.Renderers.CreateItemForm;
using Blazor.FlexGrid.Components.Renderers.CreateItemForm.Layouts;
using Blazor.FlexGrid.DataSet.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Threading;

namespace Blazor.FlexGrid.Components
{
    public class CreateItemForm<TModel, TOutputDto> : ComponentBase
        where TModel : class
        where TOutputDto : class
    {
        private ICreateItemFormViewModel<TModel> _createItemFormViewModel;

        [Inject]
        private CreateItemFormRenderer<TModel> CreateItemFormRenderer { get; set; }

        [Inject]
        private ITypePropertyAccessorCache PropertyValueAccessorCache { get; set; }

        [Inject]
        private IFormLayoutProvider<TModel> LayoutProvider { get; set; }

        [Inject]
        private ICreateItemHandle<TModel, TOutputDto> CreateItemHandle { get; set; }

        [Inject]
        private FlexGridInterop FlexGridInterop { get; set; }

		[Inject]
		private IGridConfigurationProvider ConfigurationProvider { get; set; }

        [Parameter] public CreateItemContext CreateItemContext { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            var config = ConfigurationProvider.FindGridEntityConfigurationByType(typeof(TModel));
            CreateItemFormRenderer.BuildRendererTree(
                LayoutProvider.GetLayoutBuilder() ?? new SingleColumnLayout<TModel>(),
                new CreateItemRendererContext<TModel>(_createItemFormViewModel, PropertyValueAccessorCache, CreateItemContext.CreateFormCssClasses, config),
                new BlazorRendererTreeBuilder(builder));
        }

        protected override void OnParametersSet()
        {
            if (_createItemFormViewModel is null)
            {
                _createItemFormViewModel = new CreateItemFormViewModel<TModel>(CreateItemContext.CreateItemOptions);
                _createItemFormViewModel.SaveAction = async model =>
                {
                    if (string.IsNullOrEmpty(CreateItemContext.CreateItemOptions.CreateUri))
                    {
                        CreateItemContext.NotifyItemCreated(model);
                    }
                    else
                    {
                        var dto = await CreateItemHandle.CreateItem(model, CreateItemContext.CreateItemOptions, CancellationToken.None);
                        CreateItemContext.NotifyItemCreated(dto);
                    }
                    _createItemFormViewModel.ClearModel();

                    if (CreateItemContext.CreateItemOptions.CloseAfterSuccessfullySaved)
                    {
                        await FlexGridInterop.HideModal(CreateItemOptions.CreateItemModalName);
                    }

                    StateHasChanged();
                };
            }
        }
    }
}
