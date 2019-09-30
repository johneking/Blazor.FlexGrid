using System;
using System.Reflection;

namespace Blazor.FlexGrid.Components.Renderers.FormInputs
{
    public class FormField
    {
        public Type UnderlyingType { get; }

        public Type Type => Info.PropertyType;

        public PropertyInfo Info { get; }

        public bool IsNullable { get; }

        public FormField(PropertyInfo fieldInfo)
        {
            Info = fieldInfo ?? throw new ArgumentNullException(nameof(fieldInfo));
            IsNullable = Nullable.GetUnderlyingType(fieldInfo.PropertyType) != null;
            UnderlyingType = IsNullable ? Nullable.GetUnderlyingType(fieldInfo.PropertyType) : fieldInfo.PropertyType;
        }
    }
}
