using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Blazor.FlexGrid.Components.Renderers.FormInputs
{
    public class NumberInputBuilder : IFormInputRendererBuilder
    {
        private readonly EventCallbackFactory _eventCallbackFactory;

        public NumberInputBuilder()
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
                    .OpenElement(HtmlTagNames.Div, "form-field-wrapper")
                    .OpenComponent(typeof(InputNumber<>).MakeGenericType(field.Type))
                    .AddAttribute("id", $"create-form-{localColumnName}")
                    .AddAttribute("class", "edit-text-field")
                    .AddAttribute("Value", value)
                    .AddAttribute("ValueExpression", valueExpression);

                if (field.UnderlyingType == typeof(int))
                {
                    if (field.IsNullable)
                    {
                        builder
                          .AddAttribute("ValueChanged", _eventCallbackFactory.Create<int?>(this, v => actualItemContext.SetActualItemColumnValue(localColumnName, v)))
                          .CloseComponent()
                          .AddValidationMessage<int?>(valueExpression);
                    }
                    else
                    {
                        builder
                            .AddAttribute("ValueChanged", _eventCallbackFactory.Create<int>(this, v => actualItemContext.SetActualItemColumnValue(localColumnName, v)))
                            .CloseComponent()
                            .AddValidationMessage<int>(valueExpression);
                    }

                }
                else if (field.UnderlyingType == typeof(long))
                {
                    if (field.IsNullable)
                    {
                        builder
                            .AddAttribute("ValueChanged", _eventCallbackFactory.Create<long?>(this, v => actualItemContext.SetActualItemColumnValue(localColumnName, v)))
                            .CloseComponent()
                            .AddValidationMessage<long?>(valueExpression);
                    }
                    else
                    {
                        builder
                           .AddAttribute("ValueChanged", _eventCallbackFactory.Create<long>(this, v => actualItemContext.SetActualItemColumnValue(localColumnName, v)))
                           .CloseComponent()
                           .AddValidationMessage<long>(valueExpression);
                    }

                }
                else if (field.UnderlyingType == typeof(decimal))
                {
                    if (field.IsNullable)
                    {
                        builder
                        .AddAttribute("ValueChanged", _eventCallbackFactory.Create<decimal?>(this, v => actualItemContext.SetActualItemColumnValue(localColumnName, v)))
                        .CloseComponent()
                        .AddValidationMessage<decimal?>(valueExpression);
                    }
                    else
                    {
                        builder
                            .AddAttribute("ValueChanged", _eventCallbackFactory.Create<decimal>(this, v => actualItemContext.SetActualItemColumnValue(localColumnName, v)))
                            .CloseComponent()
                            .AddValidationMessage<decimal>(valueExpression);
                    }
                }
                else if (field.UnderlyingType == typeof(double))
                {
                    if (field.IsNullable)
                    {
                        builder
                            .AddAttribute("ValueChanged", _eventCallbackFactory.Create<double?>(this, v => actualItemContext.SetActualItemColumnValue(localColumnName, v)))
                            .CloseComponent()
                            .AddValidationMessage<double?>(valueExpression);
                    }
                    else
                    {
                        builder
                            .AddAttribute("ValueChanged", _eventCallbackFactory.Create<double>(this, v => actualItemContext.SetActualItemColumnValue(localColumnName, v)))
                            .CloseComponent()
                            .AddValidationMessage<double>(valueExpression);
                    }

                }
                else if (field.UnderlyingType == typeof(float))
                {
                    if (field.IsNullable)
                    {
                        builder
                            .AddAttribute("ValueChanged", _eventCallbackFactory.Create<float?>(this, v => actualItemContext.SetActualItemColumnValue(localColumnName, v)))
                            .CloseComponent()
                            .AddValidationMessage<float?>(valueExpression);
                    }
                    else
                    {
                        builder
                            .AddAttribute("ValueChanged", _eventCallbackFactory.Create<float>(this, v => actualItemContext.SetActualItemColumnValue(localColumnName, v)))
                            .CloseComponent()
                            .AddValidationMessage<float>(valueExpression);
                    }
                }

                builder.CloseElement();
            };

        }

        public bool IsSupportedDateType(Type type)
	        => SupportedTypes.Contains(type);

        private static readonly Type[] SupportedTypes = { typeof(int), typeof(long), typeof(decimal), typeof(double), typeof(float) };
        private LambdaExpression GetValueExpression(object actualItem, FormField field)
        {
            if (field.UnderlyingType == typeof(int))
            {
                if (field.IsNullable)
                {
                    return Expression.Lambda<Func<int?>>(
                            Expression.Property(
                                Expression.Constant(actualItem),
                               field.Info));
                }
                else
                {

                    return Expression.Lambda<Func<int>>(
                         Expression.Property(
                             Expression.Constant(actualItem),
                            field.Info));
                }
            }
            else if (field.UnderlyingType == typeof(long))
            {
                if (field.IsNullable)
                {
                    return Expression.Lambda<Func<long?>>(
                       Expression.Property(
                           Expression.Constant(actualItem),
                          field.Info));
                }
                else
                {
                    return Expression.Lambda<Func<long>>(
                         Expression.Property(
                             Expression.Constant(actualItem),
                            field.Info));
                }
            }
            else if (field.UnderlyingType == typeof(decimal))
            {
                if (field.IsNullable)
                {
                    return Expression.Lambda<Func<decimal?>>(
                         Expression.Property(
                             Expression.Constant(actualItem),
                            field.Info));
                }
                else
                {
                    return Expression.Lambda<Func<decimal>>(
                         Expression.Property(
                             Expression.Constant(actualItem),
                            field.Info));
                }
            }
            else if (field.UnderlyingType == typeof(double))
            {
                if (field.IsNullable)
                {
                    return Expression.Lambda<Func<double?>>(
                       Expression.Property(
                           Expression.Constant(actualItem),
                          field.Info));
                }
                else
                {
                    return Expression.Lambda<Func<double>>(
                         Expression.Property(
                             Expression.Constant(actualItem),
                            field.Info));
                }
            }
            else if (field.UnderlyingType == typeof(float))
            {
                if (field.IsNullable)
                {
                    return Expression.Lambda<Func<float?>>(
                       Expression.Property(
                           Expression.Constant(actualItem),
                          field.Info));
                }
                else
                {
                    return Expression.Lambda<Func<float>>(
                         Expression.Property(
                             Expression.Constant(actualItem),
                            field.Info));
                }
            }

            throw new ArgumentException($"{nameof(NumberInputBuilder)} does not support type {field.UnderlyingType}");
        }
    }
}
