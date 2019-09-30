using System;

namespace Blazor.FlexGrid.Components
{
    public class EditColumnContext
    {
        private readonly string _columnName;
        private readonly Action<string, object> _onChangeAction;

        public EditColumnContext(string columnName, Action<string, object> onChangeAction)
        {
            _columnName = string.IsNullOrWhiteSpace(columnName)
                ? throw new ArgumentNullException(nameof(columnName))
                : columnName;

            _onChangeAction = onChangeAction ?? throw new ArgumentNullException(nameof(onChangeAction));
        }

        public void NotifyValueHasChanged(object value)
        {
            _onChangeAction.Invoke(_columnName, value);
        }
    }
}
