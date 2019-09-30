using Microsoft.AspNetCore.Components;
using System;

namespace Blazor.FlexGrid.Components.Renderers.EditInputs
{
    public class DateTimeInputRenderer : AbstractEditInputRenderer
    {
        public override void BuildInputRendererTree(IRendererTreeBuilder rendererTreeBuilder, IActualItemContext<object> actualItemContext, Action<string, object> onChangeAction, string columnName)
        {
            var value = actualItemContext.GetActualItemColumnValue(columnName);
            if (IsSupportedDateType(value))
            {
                var dateTimeValue = ConvertToDateTime(value);
                var dateValueContainsTime = Math.Abs(dateTimeValue.TimeOfDay.TotalSeconds) > 0.00000000001;
                var dateFormat = dateValueContainsTime ? "yyyy-MM-dd'T'HH:mm:ss" : "yyyy-MM-dd";

                rendererTreeBuilder
                    .OpenElement(HtmlTagNames.Div, "edit-field-wrapper")
                    .OpenElement(HtmlTagNames.Input, "edit-text-field")
                    .AddAttribute(HtmlAttributes.Type, dateValueContainsTime ? "datetime-local" : "date")
                    .AddAttribute(HtmlAttributes.Value, BindConverter.FormatValue(dateTimeValue, dateFormat))
                    .AddAttribute(HtmlJsEvents.OnChange, EventCallback.Factory.Create(this,
                        (ChangeEventArgs e) =>
                        {
                            onChangeAction?.Invoke(columnName, BindConverterExtensions.ConvertTo(e.Value, DateTime.MinValue));
                        }))
                    .CloseElement()
                    .CloseElement();
            }
            else
            {
                Successor.BuildInputRendererTree(rendererTreeBuilder, actualItemContext, onChangeAction, columnName);
            }
        }

        private static bool IsSupportedDateType(object value)
            => value is DateTime || value is DateTimeOffset;

        private static DateTime ConvertToDateTime(object value)
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset.DateTime;
            }

            if (value is DateTime dateTime)
            {
                return dateTime;
            }

            return DateTime.Now;
        }
    }
}
