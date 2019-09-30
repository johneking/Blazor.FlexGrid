using Microsoft.AspNetCore.Components;
using System;

namespace Blazor.FlexGrid.Components.Renderers.EditInputs
{
    public class NumberInputType : AbstractEditInputRenderer
    {
        public override void BuildInputRendererTree(IRendererTreeBuilder rendererTreeBuilder, IActualItemContext<object> actualItemContext, Action<string, object> onChangeAction, string columnName)
        {
            var value = actualItemContext.GetActualItemColumnValue(columnName);
            if (IsSupportedNumberType(value))
            {
                rendererTreeBuilder
                    .OpenElement(HtmlTagNames.Div, "edit-field-wrapper")
                    .OpenElement(HtmlTagNames.Input, "edit-text-field")
                    .AddAttribute(HtmlAttributes.Type, "number");

                TryAddOnChangeHandler(rendererTreeBuilder, onChangeAction, columnName, value);

                rendererTreeBuilder
                    .CloseElement()
                    .CloseElement();
            }
            else
            {
                Successor.BuildInputRendererTree(rendererTreeBuilder, actualItemContext, onChangeAction, columnName);
            }
        }

        private bool IsSupportedNumberType(object value)
            => value is int || value is long || value is decimal || value is double;

        private void TryAddOnChangeHandler(IRendererTreeBuilder rendererTreeBuilder, Action<string, object> onChangeAction, string localColumnName, object value)
        {
            if (value is int intValue)
            {
                rendererTreeBuilder.AddAttribute(HtmlAttributes.Value, BindConverter.FormatValue(intValue));
                rendererTreeBuilder.AddAttribute(HtmlJsEvents.OnChange, EventCallback.Factory.Create(this,
                    (ChangeEventArgs e) => onChangeAction?.Invoke(localColumnName, BindConverterExtensions.ConvertTo(e.Value, 0))));
            }
            else
            {
                AddLongValueHandlerIfValueIsLong(rendererTreeBuilder, onChangeAction, localColumnName, value);
            }
        }

        private void AddLongValueHandlerIfValueIsLong(IRendererTreeBuilder rendererTreeBuilder, Action<string, object> onChangeAction, string localColumnName, object value)
        {
            if (value is long longValue)
            {
                rendererTreeBuilder.AddAttribute(HtmlAttributes.Value, BindConverter.FormatValue(longValue));
                rendererTreeBuilder.AddAttribute(HtmlJsEvents.OnChange, EventCallback.Factory.Create(this,
                    (ChangeEventArgs e) => onChangeAction?.Invoke(localColumnName, BindConverterExtensions.ConvertTo(e.Value, 0L))));
            }
            else
            {
                AddDecimalValueHandlerIfValueIsDecimal(rendererTreeBuilder, onChangeAction, localColumnName, value);
            }
        }

        private void AddDecimalValueHandlerIfValueIsDecimal(IRendererTreeBuilder rendererTreeBuilder, Action<string, object> onChangeAction, string localColumnName, object value)
        {
            if (value is decimal decimalValue)
            {
                rendererTreeBuilder.AddAttribute(HtmlAttributes.Value, BindConverter.FormatValue(decimalValue));
                rendererTreeBuilder.AddAttribute(HtmlJsEvents.OnChange, EventCallback.Factory.Create(this,
                    (ChangeEventArgs e) => onChangeAction?.Invoke(localColumnName, BindConverterExtensions.ConvertTo(e.Value, 0m))));
            }
            else
            {
                AddDoubleValueHandlerIfValueIsDouble(rendererTreeBuilder, onChangeAction, localColumnName, value);
            }
        }

        private void AddDoubleValueHandlerIfValueIsDouble(IRendererTreeBuilder rendererTreeBuilder, Action<string, object> onChangeAction, string localColumnName, object value)
        {
            if (value is double doubleValue)
            {
                rendererTreeBuilder.AddAttribute(HtmlAttributes.Value, BindConverter.FormatValue(doubleValue));
                rendererTreeBuilder.AddAttribute(HtmlJsEvents.OnChange, EventCallback.Factory.Create(this,
                    (ChangeEventArgs e) => onChangeAction?.Invoke(localColumnName, BindConverterExtensions.ConvertTo(e.Value, 0d))));
            }
        }
    }
}
