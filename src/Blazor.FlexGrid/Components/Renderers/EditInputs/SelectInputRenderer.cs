using Microsoft.AspNetCore.Components;
using System;

namespace Blazor.FlexGrid.Components.Renderers.EditInputs
{
    public class SelectInputRenderer : AbstractEditInputRenderer
    {
        public override void BuildInputRendererTree(IRendererTreeBuilder rendererTreeBuilder, IActualItemContext<object> actualItemContext, Action<string, object> onChangeAction, string columnName)
        {
            var value = actualItemContext.GetActualItemColumnValue(columnName);
            if (value is Enum enumTypeValue)
            {
                var actualStringValue = enumTypeValue.ToString();

                rendererTreeBuilder
                    .OpenElement(HtmlTagNames.Div, "edit-field-wrapper")
                    .OpenElement(HtmlTagNames.Select, "edit-text-field")
                    .AddAttribute(HtmlJsEvents.OnChange, EventCallback.Factory.Create(this,
                        (ChangeEventArgs e) =>
                        {
                            var parsedValue = Enum.Parse(value.GetType(), e.Value.ToString());
                            onChangeAction?.Invoke(columnName, parsedValue);
                        }
                    ));

                foreach (var enumValue in Enum.GetValues(enumTypeValue.GetType()))
                {
                    var enumStringValue = enumValue.ToString();

                    rendererTreeBuilder.OpenElement(HtmlTagNames.Option);
                    if (enumStringValue == actualStringValue)
                    {
                        rendererTreeBuilder.AddAttribute(HtmlAttributes.Selected, true);
                    }

                    rendererTreeBuilder
                        .AddAttribute(HtmlAttributes.Value, enumStringValue)
                        .AddContent(enumStringValue)
                        .CloseElement();
                }

                rendererTreeBuilder
                    .CloseElement()
                    .CloseElement();
            }
            else
            {
                Successor.BuildInputRendererTree(rendererTreeBuilder, actualItemContext, onChangeAction, columnName);
            }
        }
    }
}
