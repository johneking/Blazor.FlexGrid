using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Blazor.FlexGrid.Components.Renderers.FormInputs
{
    public class CheckBoxInputBuilder : IFormInputRendererBuilder
    {
        private readonly EventCallbackFactory _eventCallbackFactory;

        public CheckBoxInputBuilder()
        {
            _eventCallbackFactory = new EventCallbackFactory();
        }

        public Action<IRendererTreeBuilder> BuildRendererTree<TItem>(IActualItemContext<TItem> actualItemContext, FormField field, string localColumnName) where TItem : class
        {
            var value = actualItemContext.GetActualItemColumnValue(localColumnName);

            var valueExpression = GetValueExpression(actualItemContext.ActualItem, field);

            return builder =>
            {
                builder
                    .OpenElement(HtmlTagNames.Div, "checkbox")
                    .OpenElement(HtmlTagNames.Label)
                    .OpenComponent(typeof(InputCheckbox))
                    .AddAttribute("id", $"create-form-{localColumnName}")
                    .AddAttribute("class", string.Empty)
                    .AddAttribute("Value", value)
                    .AddAttribute("ValueExpression", valueExpression);

                if (field.IsNullable)
                {
                    builder.AddAttribute("ValueChanged", _eventCallbackFactory.Create<bool?>(this, v => actualItemContext.SetActualItemColumnValue(localColumnName, v)));
                }
                else
                {
                    builder.AddAttribute("ValueChanged", _eventCallbackFactory.Create<bool>(this, v => actualItemContext.SetActualItemColumnValue(localColumnName, v)));
                }

                builder
                    .CloseComponent()
                    .OpenElement(HtmlTagNames.Span, "cr")
                    .OpenElement(HtmlTagNames.I, "cr-icon fa fa-check")
                    .CloseElement()
                    .CloseElement()
                    .CloseElement()
                    .CloseElement();

                if (field.IsNullable)
                {
                    builder.AddValidationMessage<bool?>(valueExpression);
                }
                else
                {
                    builder.AddValidationMessage<bool>(valueExpression);
                }
            };
        }

        public bool IsSupportedDateType(Type type)
            => SupportedTypes.Contains(type);

		private static readonly Type[] SupportedTypes ={ typeof(bool) };

        private LambdaExpression GetValueExpression(object actualItem, FormField field)
        {
            if (field.IsNullable)
            {
                return Expression.Lambda<Func<bool?>>(
                 Expression.Property(
                     Expression.Constant(actualItem),
                    field.Info));
            }

            return Expression.Lambda<Func<bool>>(
                 Expression.Property(
                     Expression.Constant(actualItem),
                    field.Info));
        }
    }
}
