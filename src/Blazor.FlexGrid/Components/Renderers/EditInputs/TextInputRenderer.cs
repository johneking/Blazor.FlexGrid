using Microsoft.AspNetCore.Components;
using System;

namespace Blazor.FlexGrid.Components.Renderers.EditInputs
{
    public class TextInputRenderer : AbstractEditInputRenderer
    {
        private const string InputTypeText = "text";
        private const string InputTypeEmail = "email";

        public override void BuildInputRendererTree(IRendererTreeBuilder rendererTreeBuilder, IActualItemContext<object> actualItemContext, Action<string, object> onChangeAction, string columnName)
        {
            var value = actualItemContext.GetActualItemColumnValue(columnName);

            rendererTreeBuilder
                .OpenElement(HtmlTagNames.Div, "edit-field-wrapper")
                .OpenElement(HtmlTagNames.Input, "edit-text-field")
                .AddAttribute(HtmlAttributes.Type, GetInputType(value?.ToString() ?? InputTypeText))
                .AddAttribute(HtmlAttributes.Value, BindConverter.FormatValue(value))
                .AddAttribute(HtmlJsEvents.OnChange, EventCallback.Factory.Create(this,
                    (ChangeEventArgs e) => onChangeAction?.Invoke(columnName, BindConverterExtensions.ConvertTo(e.Value, string.Empty)))
                )
                .CloseElement()
                .CloseElement();
        }

        private static string GetInputType(string value)
        {
	        return value.Contains("@") ? InputTypeEmail : InputTypeText;
        }
    }
}
