using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;

namespace Blazor.FlexGrid.Components.Renderers
{
    public class BlazorRendererTreeBuilder : IRendererTreeBuilder
    {
        public const string ChildContent = "ChildContent";

        private readonly RenderTreeBuilder _renderTreeBuilder;
        private int _sequence;

        public BlazorRendererTreeBuilder(RenderTreeBuilder renderTreeBuilder)
        {
            _renderTreeBuilder = renderTreeBuilder ?? throw new ArgumentNullException(nameof(renderTreeBuilder));
        }

        public IRendererTreeBuilder AddAttribute(string name, object value)
        {
            _renderTreeBuilder.AddAttribute(++_sequence, name, value);

            return this;
        }

        public IRendererTreeBuilder AddAttribute(string name, bool value)
        {
            _renderTreeBuilder.AddAttribute(++_sequence, name, value);

            return this;
        }

        public IRendererTreeBuilder AddAttribute(string name, string value)
        {
            _renderTreeBuilder.AddAttribute(++_sequence, name, value);

            return this;
        }

        public IRendererTreeBuilder AddAttribute(string name, MulticastDelegate value)
        {
            _renderTreeBuilder.AddAttribute(++_sequence, name, value);

            return this;
        }

        public IRendererTreeBuilder AddAttribute(string name, Action<ChangeEventArgs> value)
        {
            _renderTreeBuilder.AddAttribute(++_sequence, name, value);

            return this;
        }

        public IRendererTreeBuilder AddAttribute(string name, Func<MulticastDelegate> value)
        {
            _renderTreeBuilder.AddAttribute(++_sequence, name, value);

            return this;
        }

        public IRendererTreeBuilder AddContent(string textContent)
        {
            _renderTreeBuilder.AddContent(++_sequence, textContent);

            return this;
        }

        public IRendererTreeBuilder AddContent(RenderFragment fragment)
        {
            _renderTreeBuilder.AddContent(++_sequence, fragment);

            return this;
        }

        public IRendererTreeBuilder AddContent(MarkupString markupContent)
        {
            _renderTreeBuilder.AddContent(++_sequence, markupContent);

            return this;
        }

        public IRendererTreeBuilder CloseComponent()
        {
            _renderTreeBuilder.CloseComponent();

            return this;
        }

        public IRendererTreeBuilder CloseElement()
        {
            _renderTreeBuilder.CloseElement();

            return this;
        }

        public IRendererTreeBuilder OpenComponent(Type componentType)
        {
            _renderTreeBuilder.OpenComponent(++_sequence, componentType);

            return this;
        }

        public IRendererTreeBuilder OpenElement(string elementName)
        {
            _renderTreeBuilder.OpenElement(++_sequence, elementName);

            return this;
        }
    }
}
