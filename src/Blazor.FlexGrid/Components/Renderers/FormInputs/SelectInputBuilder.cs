using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Blazor.FlexGrid.Components.Renderers.FormInputs
{
    public class SelectInputBuilder : IFormInputRendererBuilder
    {
        private static readonly Dictionary<Type, SelectInputContext> SelectInputContexts = new Dictionary<Type, SelectInputContext>();

        private readonly MethodInfo _callbackMethodBuilderMethodInfo;
        private readonly MethodInfo _valueChangedMethodInfo;

        public SelectInputBuilder()
        {
            _callbackMethodBuilderMethodInfo = typeof(ReflectionHelper).GetMethod(nameof(ReflectionHelper.CreateEventCallback));
            _valueChangedMethodInfo = typeof(SelectInputBuilder).GetMethod(nameof(ValueChanged));
        }

        public Action<IRendererTreeBuilder> BuildRendererTree<TItem>(IActualItemContext<TItem> actualItemContext, FormField field, string localColumnName) where TItem : class
        {
            var localValue = actualItemContext.GetActualItemColumnValue(localColumnName);

            var valueExpression = Expression.Lambda(Expression.Property(Expression.Constant(actualItemContext.ActualItem), field.Info));
            var optionsFragment = CreateOptionsFragment(localValue);
            var valueChanged = CreateValueChangedCallback(field.Info);

            InitializeSelectInputContext(actualItemContext, field.Info, localColumnName);

            return builder =>
            {
                builder
                    .OpenElement(HtmlTagNames.Div, "form-field-wrapper")
                    .OpenComponent(typeof(InputSelect<>).MakeGenericType(field.UnderlyingType))
                    .AddAttribute("id", $"create-form-{localColumnName}")
                    .AddAttribute("class", "edit-text-field")
                    .AddAttribute("Value", localValue)
                    .AddAttribute("ValueExpression", valueExpression)
                    .AddAttribute(BlazorRendererTreeBuilder.ChildContent, optionsFragment)
                    .AddAttribute("ValueChanged", valueChanged)
                    .CloseComponent()
                    .OpenComponent(typeof(ValidationMessage<>).MakeGenericType(field.UnderlyingType))
                    .AddAttribute("For", valueExpression)
                    .CloseComponent()
                    .CloseElement();
            };
        }


        public bool IsSupportedDateType(Type type)
            => type.IsEnum;

        public static void ValueChanged<T>(T value)
        {
            if (SelectInputContexts.TryGetValue(value.GetType(), out var context))
            {
                context.ValueChangedAction.Invoke(value);
            }
        }

        private void InitializeSelectInputContext<TItem>(IActualItemContext<TItem> actualItemContext, PropertyInfo field, string localColumnName)
            where TItem : class
        {
            if (SelectInputContexts.TryGetValue(field.PropertyType, out var context))
            {
                context.SetValueChangedAction(value =>
                    actualItemContext.SetActualItemColumnValue(localColumnName, Enum.Parse(field.PropertyType, value.ToString()))
                );
            }
            else
            {
                var newContext = new SelectInputContext();
                newContext.SetValueChangedAction(value =>
                    actualItemContext.SetActualItemColumnValue(localColumnName, Enum.Parse(field.PropertyType, value.ToString()))
                );

                SelectInputContexts.Add(field.PropertyType, newContext);
            }
        }

        private RenderFragment CreateOptionsFragment(object value)
        {
            return delegate (RenderTreeBuilder internalBuilder)
            {
                var rendererTreeBuilder = new BlazorRendererTreeBuilder(internalBuilder);

                if (value is Enum enumTypeValue)
                {
                    foreach (var enumValue in Enum.GetValues(enumTypeValue.GetType()))
                    {
                        var enumStringValue = enumValue.ToString();

                        rendererTreeBuilder.OpenElement(HtmlTagNames.Option);
                        if (enumStringValue == value.ToString())
                        {
                            rendererTreeBuilder.AddAttribute(HtmlAttributes.Selected, true);
                        }

                        rendererTreeBuilder
                            .AddAttribute(HtmlAttributes.Value, enumStringValue)
                            .AddContent(enumStringValue)
                            .CloseElement();
                    }
                }
            };
        }

        private object CreateValueChangedCallback(PropertyInfo field)
        {
            var callbackMethodBuilder = _callbackMethodBuilderMethodInfo.MakeGenericMethod(field.PropertyType);
            var actionType = typeof(Action<>).MakeGenericType(field.PropertyType);
            var typedValueChangeMethod = _valueChangedMethodInfo.MakeGenericMethod(field.PropertyType);
            var @delegate = Delegate.CreateDelegate(actionType, typedValueChangeMethod);
            var valueChanged = callbackMethodBuilder.Invoke(null, new object[] { this, @delegate });

            return valueChanged;
        }
    }

    internal class SelectInputContext
    {
        public Action<object> ValueChangedAction { get; private set; }

        public void SetValueChangedAction(Action<object> action)
        {
            ValueChangedAction = action;
        }
    }
}
