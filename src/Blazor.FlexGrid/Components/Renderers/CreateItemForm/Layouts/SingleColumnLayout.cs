using Blazor.FlexGrid.Components.Renderers.FormInputs;
using System;

namespace Blazor.FlexGrid.Components.Renderers.CreateItemForm.Layouts
{
    public class SingleColumnLayout<TModel> : BaseCreateItemFormLayout<TModel> where TModel : class
    {
        public override Action<IRendererTreeBuilder> BuildBodyRendererTree(
            CreateItemRendererContext<TModel> createItemRendererContext, // todo - add wrapped properties to this with annotation info
            IFormInputRendererTreeProvider formInputRendererTreeProvider)
        {
            return builder =>
            {
                builder.OpenElement(HtmlTagNames.Div, "center-block");

                foreach (var field in createItemRendererContext.GetModelFields())
                {
                    //var configProperty = entityType.FindProperty(field.Name);
                    //if(configProperty == null || configProperty.IsEditable)
                    BuildFormFieldRendererTree(field, createItemRendererContext, formInputRendererTreeProvider, field.Name)?.Invoke(builder);
                }

                builder.CloseElement();
            };
        }
    }
}
