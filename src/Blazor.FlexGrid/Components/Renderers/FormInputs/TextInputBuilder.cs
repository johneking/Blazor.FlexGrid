using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Linq.Expressions;

namespace Blazor.FlexGrid.Components.Renderers.FormInputs
{
    public class TextInputBuilder : IFormInputRendererBuilder
    {
        private readonly EventCallbackFactory _eventCallbackFactory;

        public TextInputBuilder()
        {
            _eventCallbackFactory = new EventCallbackFactory();
        }

        public Action<IRendererTreeBuilder> BuildRendererTree<TItem>(IActualItemContext<TItem> actualItemContext, FormField field, string columnName) where TItem : class
        {
            var value = actualItemContext.GetActualItemColumnValue(columnName);

            var valueExpression = Expression.Lambda<Func<string>>(
                 Expression.Property(
                     Expression.Constant(actualItemContext.ActualItem),
                    field.Info));

            return builder =>
            {
                builder
                    .OpenElement(HtmlTagNames.Div, "form-field-wrapper")
                    .OpenComponent(typeof(InputText))
                    .AddAttribute("id", $"create-form-{columnName}")
                    .AddAttribute("class", "edit-text-field")
                    .AddAttribute("Value", value)
                    .AddAttribute("ValueExpression", valueExpression)
                    .AddAttribute("ValueChanged", _eventCallbackFactory.Create<string>(this, v => actualItemContext.SetActualItemColumnValue(columnName, v)))
                    .CloseComponent()
                    .AddValidationMessage<string>(valueExpression)
                    .CloseElement();
            };
        }

        public bool IsSupportedDateType(Type type)
            => type == typeof(string);
    }
}
