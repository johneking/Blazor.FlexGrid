using Blazor.FlexGrid.Components.Configuration.ValueFormatters;
using Blazor.FlexGrid.Components.Renderers;
using Blazor.FlexGrid.Permission;
using System;

namespace Blazor.FlexGrid.Components.Configuration.MetaData
{
	public class GridColumnAnnotations : IGridViewColumnAnnotations
    {
        public const int DefaultOrder = 5000;

        private readonly IProperty _propertyMetadata;

        public string Caption
        {
            get
            {
                var captionAnnotationValue = Annotations[GridViewAnnotationNames.ColumnCaption];
                if (captionAnnotationValue is NullAnnotationValue)
                {
                    return _propertyMetadata.Name;
                }

                return captionAnnotationValue.ToString();
            }
        }

        public string HeaderStyle
        {
            get
            {
                var captionAnnotationValue = Annotations[GridViewAnnotationNames.ColumnHeaderStyle];
                if (captionAnnotationValue is NullAnnotationValue)
                {
                    return string.Empty;
                }

                return captionAnnotationValue.ToString();
            }
        }

        public int Order
        {
            get
            {
                var orderAnnotationValue = Annotations[GridViewAnnotationNames.ColumnOrder];
                if (orderAnnotationValue is NullAnnotationValue)
                {
                    return DefaultOrder;
                }

                return (int)orderAnnotationValue;
            }
        }

        public bool IsVisible
        {
            get
            {
                var orderAnnotationValue = Annotations[GridViewAnnotationNames.ColumnIsVisible];
                if (orderAnnotationValue is NullAnnotationValue)
                {
                    return true;
                }

                return (bool)orderAnnotationValue;
            }
        }

        public bool IsSortable
        {
            get
            {
                var sortAnnotationValue = Annotations[GridViewAnnotationNames.ColumnIsSortable];
                if (sortAnnotationValue is NullAnnotationValue)
                {
                    return false;
                }

                return (bool)sortAnnotationValue;
            }
        }

        public bool IsFilterable
        {
            get
            {
                var filterAnnotationValue = Annotations[GridViewAnnotationNames.ColumnIsFilterable];
                if (filterAnnotationValue is NullAnnotationValue)
                {
                    return false;
                }

                return (bool)filterAnnotationValue;
            }
        }


        public IValueFormatter ValueFormatter
        {
            get
            {
                var formatterValueAnnotation = Annotations[GridViewAnnotationNames.ColumnValueFormatter];
                if (formatterValueAnnotation is NullAnnotationValue)
                {
                    return new DefaultValueFormatter();
                }

                return formatterValueAnnotation as IValueFormatter;
            }
        }

        public IRenderFragmentAdapter SpecialColumnValue
        {
            get
            {
                var specialColumnValue = Annotations[GridViewAnnotationNames.ColumnValueBlazorComponent];
                if (specialColumnValue is NullAnnotationValue)
                {
                    return null;
                }

                return specialColumnValue as IRenderFragmentAdapter;
            }
        }

        public Func<EditColumnContext, IRenderFragmentAdapter> ColumnEditComponentBuilder
        {
            get
            {
                var edcitComponentFragmentBuilder = Annotations[GridViewAnnotationNames.ColumnEditBlazorComponentBuilder];
                if (edcitComponentFragmentBuilder is NullAnnotationValue)
                {
                    return null;
                }

                return edcitComponentFragmentBuilder as Func<EditColumnContext, IRenderFragmentAdapter>;
            }
        }

        public Func<ICurrentUserPermission, bool> ReadPermissionRestrictionFunc
        {
            get
            {
                var hasPermissionFunc = Annotations[GridViewAnnotationNames.ColumnReadPermission];
                if (hasPermissionFunc is NullAnnotationValue)
                {
                    return p => true;
                }

                return hasPermissionFunc as Func<ICurrentUserPermission, bool>;
            }
        }

        public Func<ICurrentUserPermission, bool> WritePermissionRestrictionFunc
        {
            get
            {
                var hasPermissionFunc = Annotations[GridViewAnnotationNames.ColumnWritePermission];
                if (hasPermissionFunc is NullAnnotationValue)
                {
                    return p => true;
                }

                return hasPermissionFunc as Func<ICurrentUserPermission, bool>;
            }
        }

        protected IAnnotatable Annotations { get; }

        public GridColumnAnnotations(IProperty property)
        {
            Annotations = property ?? throw new ArgumentNullException(nameof(property));
            _propertyMetadata = property;
        }
    }
}
