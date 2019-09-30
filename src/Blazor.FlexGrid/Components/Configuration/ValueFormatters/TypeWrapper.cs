using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Blazor.FlexGrid.Components.Configuration.ValueFormatters
{
    public class TypeWrapper : ITypePropertyAccessor
    {
        private readonly Dictionary<string, Func<object, object>> _getters
            = new Dictionary<string, Func<object, object>>();

        private readonly Dictionary<string, Action<object, object>> _setters
            = new Dictionary<string, Action<object, object>>();

        private readonly List<PropertyInfo> _properties;

        private readonly ILogger _logger;

        public IEnumerable<PropertyInfo> Properties => _properties;

		
        public TypeWrapper(Type clrType, ILogger logger)
        {
            _properties = new List<PropertyInfo>();

            foreach (var property in clrType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                _properties.Add(property);

                var wrappedObjectParameter = Expression.Parameter(typeof(object));
                var valueParameter = Expression.Parameter(typeof(object));

                var getExpression = Expression.Lambda<Func<object, object>>(
                    Expression.Convert(
                        Expression.Property(
                            Expression.Convert(wrappedObjectParameter, clrType), property),
                        typeof(object)),
                    wrappedObjectParameter);

                _getters.Add(property.Name, getExpression.Compile());

                if (property.CanWrite)
                {

                    var setExpression = Expression.Lambda<Action<object, object>>(
                         Expression.Assign(
                             Expression.Property(
                                 Expression.Convert(wrappedObjectParameter, clrType), property),
                             Expression.Convert(valueParameter, property.PropertyType)),
                         wrappedObjectParameter, valueParameter);

                    _setters.Add(property.Name, setExpression.Compile());
                }
            }

            _logger = logger;
        }

        public object GetValue(object @object, string name)
        {
            try
            {
                var get = _getters[name];
                return get(@object);
            }
            catch (Exception ex)
            {
                _logger.LogError($"TypeWrapper:GetValue. Ex: {ex}");

                throw;
            }
        }

        public void SetValue(object instance, string propertyName, object value)
        {
            try
            {
                var set = _setters[propertyName];
                set(instance, value);
            }

            catch (Exception ex)
            {
                _logger.LogError($"TypeWrapper:SetValue. Ex: {ex}");

                throw;
            }
        }
    }
}
