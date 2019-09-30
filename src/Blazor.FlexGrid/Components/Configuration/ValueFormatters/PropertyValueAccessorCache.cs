using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Blazor.FlexGrid.Components.Configuration.ValueFormatters
{
    public class PropertyValueAccessorCache : ITypePropertyAccessorCache
    {
        private readonly Dictionary<Type, ITypePropertyAccessor> _propertyAccessors;
        private readonly ILogger<PropertyValueAccessorCache> _logger;

        public PropertyValueAccessorCache(ILogger<PropertyValueAccessorCache> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _propertyAccessors = new Dictionary<Type, ITypePropertyAccessor>();
        }

        public void AddPropertyAccessor(Type type, ITypePropertyAccessor propertyValueAccessor)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (propertyValueAccessor is null)
            {
                throw new ArgumentNullException(nameof(propertyValueAccessor));
            }

            if (_propertyAccessors.ContainsKey(type))
            {
                return;
            }

            _propertyAccessors.Add(type, propertyValueAccessor);
        }

        public ITypePropertyAccessor GetPropertyAccesor(Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (_propertyAccessors.TryGetValue(type, out var propertyValueAccessor))
            {
                return propertyValueAccessor;
            }

            propertyValueAccessor = new TypeWrapper(type, _logger);
            _propertyAccessors.Add(type, propertyValueAccessor);

            return propertyValueAccessor;
        }
    }
}
