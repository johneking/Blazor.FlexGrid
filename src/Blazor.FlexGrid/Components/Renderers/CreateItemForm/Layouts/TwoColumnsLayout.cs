using Blazor.FlexGrid.Components.Renderers.FormInputs;
using System;
using System.Linq;
using System.Reflection;

namespace Blazor.FlexGrid.Components.Renderers.CreateItemForm.Layouts
{
    public class TwoColumnsLayout<TModel> : BaseCreateItemFormLayout<TModel> where TModel : class
    {
        private const int ColumnsCount = 2;

        public override Action<IRendererTreeBuilder> BuildBodyRendererTree(
            CreateItemRendererContext<TModel> createItemRendererContext,
            IFormInputRendererTreeProvider formInputRendererTreeProvider)
        {
            return builder =>
            {
                var fields = createItemRendererContext.GetModelFields().ToList();

                builder.OpenElement(HtmlTagNames.Div, "center-block");

                for (int i = 0; i < fields.Count; i = i + ColumnsCount)
                {
                    builder.OpenElement(HtmlTagNames.Div, "form-group row");

                    var rowFields = fields.Skip(i).Take(ColumnsCount);
                    foreach (var field in rowFields)
                    {
	                    //var configProperty = entityType.FindProperty(field.Name);
	                    //if (configProperty == null || configProperty.IsEditable)
                            BuildFormFieldRendererTree(field, createItemRendererContext, formInputRendererTreeProvider, field.Name)?.Invoke(builder);
                    }

                    builder.CloseElement();
                }

                builder.CloseElement();
            };
        }

        public override Action<IRendererTreeBuilder> BuildFieldRendererTree(
            PropertyInfo field,
            CreateItemRendererContext<TModel> createItemRendererContext,
            IFormInputRendererBuilder formInputRendererBuilder, 
            string columnName)
        {
            return builder =>
            {
                builder
                    .OpenElement(HtmlTagNames.Div, "col-sm-6")
                    .OpenElement(HtmlTagNames.Label, createItemRendererContext.CreateFormCssClasses.FieldName)
                    .AddContent(field.Name)
                    .CloseElement();

                formInputRendererBuilder.BuildRendererTree(createItemRendererContext, new FormField(field), columnName)?.Invoke(builder);

                builder.CloseElement();
            };
        }
    }
}
