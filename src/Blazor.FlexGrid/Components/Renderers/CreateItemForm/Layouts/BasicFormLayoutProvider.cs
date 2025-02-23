﻿using Blazor.FlexGrid.Components.Configuration.ValueFormatters;
using System;
using System.Linq;

namespace Blazor.FlexGrid.Components.Renderers.CreateItemForm.Layouts
{
    public class BasicFormLayoutProvider<TModel> : IFormLayoutProvider<TModel> where TModel : class
    {
        private const int MaximumFieldsForSingleColumnLayout = 6;

        private readonly ITypePropertyAccessorCache _typePropertyAccessorCache;

        public BasicFormLayoutProvider(ITypePropertyAccessorCache typePropertyAccessorCache)
        {
            _typePropertyAccessorCache = typePropertyAccessorCache ?? throw new ArgumentNullException(nameof(typePropertyAccessorCache));
        }

        public IFormLayout<TModel> GetLayoutBuilder()
        {
            var modelType = typeof(TModel);
            var typePropertyAccessor = _typePropertyAccessorCache.GetPropertyAccesor(modelType);
            var fieldsCount = typePropertyAccessor.Properties.Count(p => p.CanWrite);
            if (MaximumFieldsForSingleColumnLayout >= fieldsCount)
            {
                return new SingleColumnLayout<TModel>();
            }

            return new TwoColumnsLayout<TModel>();
        }
    }
}
