using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.FlexGrid.Components.Renderers.FormInputs
{
    public class FormInputsRendererTreeProvider : IFormInputRendererTreeProvider
    {
        private readonly IEnumerable<IFormInputRendererBuilder> _formInputRendererBuilders;

        public FormInputsRendererTreeProvider(IEnumerable<IFormInputRendererBuilder> formInputRendererBuilders)
        {
            _formInputRendererBuilders = formInputRendererBuilders ?? throw new ArgumentException(nameof(formInputRendererBuilders));
        }

        public IFormInputRendererBuilder GetFormInputRendererTreeBuilder(FormField formField)
        {
	        var builder = _formInputRendererBuilders.FirstOrDefault(b => b.IsSupportedDateType(formField.UnderlyingType));
            if (builder is null)
            {
                throw new InvalidOperationException($"Type {formField.Type.FullName} is not supported in edit forms");
            }

            return builder;
        }
    }
}
