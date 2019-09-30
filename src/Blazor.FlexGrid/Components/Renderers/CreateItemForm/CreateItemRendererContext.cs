using Blazor.FlexGrid.Components.Configuration;
using Blazor.FlexGrid.Components.Configuration.ValueFormatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Blazor.FlexGrid.Components.Configuration.MetaData;

namespace Blazor.FlexGrid.Components.Renderers.CreateItemForm
{
    public class CreateItemRendererContext<TModel> : IActualItemContext<TModel> where TModel : class
    {
        private readonly ITypePropertyAccessor _typePropertyAccessor;

        public TModel ActualItem => ViewModel.Model;

        public ICreateItemFormViewModel<TModel> ViewModel { get; }

        public CreateFormCssClasses CreateFormCssClasses { get; }

        private readonly Dictionary<string, INewItemAnnotations> _annotationLookup;
        public CreateItemRendererContext(
            ICreateItemFormViewModel<TModel> createItemFormViewModel,
            ITypePropertyAccessorCache typePropertyAccessorCache,
            CreateFormCssClasses createFormCssClasses, 
            IEntityType entityConfiguration)
        {
            _typePropertyAccessor = typePropertyAccessorCache?.GetPropertyAccesor(typeof(TModel))
                ?? throw new ArgumentNullException(nameof(typePropertyAccessorCache));

            ViewModel = createItemFormViewModel ?? throw new ArgumentNullException(nameof(createItemFormViewModel));
            _annotationLookup = entityConfiguration.GetProperties()
	            .ToDictionary(p => p.Name, p => (INewItemAnnotations) new NewItemAnnotations(p));
            CreateFormCssClasses = createFormCssClasses ?? new DefaultCreateFormCssClasses();
        }

        public object GetActualItemColumnValue(string columnName)
            => _typePropertyAccessor.GetValue(ViewModel.Model, columnName);

        public void SetActualItemColumnValue(string columnName, object value)
            => _typePropertyAccessor.SetValue(ViewModel.Model, columnName, value);

        public IEnumerable<PropertyInfo> GetModelFields()
            => _typePropertyAccessor.Properties.Where(p => p.CanWrite && _annotationLookup.ContainsKey(p.Name) && _annotationLookup[p.Name].IsEditable);
    }
}
