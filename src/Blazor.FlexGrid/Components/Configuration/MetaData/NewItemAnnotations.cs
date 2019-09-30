using System;

namespace Blazor.FlexGrid.Components.Configuration.MetaData
{
	public class NewItemAnnotations : INewItemAnnotations
	{
		protected IAnnotatable Annotations { get; }
		public NewItemAnnotations(IProperty property)
		{
			Annotations = property ?? throw new ArgumentNullException(nameof(property));
		}
		public bool IsEditable
		{
			get
			{
				var isEditableValue = Annotations[NewItemAnnotationNames.FieldIsEditable];
				if (isEditableValue is NullAnnotationValue)
				{
					return true;
				}

				return (bool)isEditableValue;
			}
		}

		public object GetDefault()
		{
			var getDefaultValue = Annotations[NewItemAnnotationNames.GetDefaultValueDelegate];
			if (getDefaultValue is NullAnnotationValue)
			{
				return null;
			}

			return ((Func<object>)getDefaultValue)();
        }
	}
}