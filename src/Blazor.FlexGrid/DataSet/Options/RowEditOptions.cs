using System.Collections.Generic;

namespace Blazor.FlexGrid.DataSet.Options
{
    public class RowEditOptions : IRowEditOptions
    {
        private readonly Dictionary<string, object> _updatedValues;

        public object ItemInEditMode { get; set; } = EmptyDataSetItem.Instance;

        public IReadOnlyDictionary<string, object> UpdatedValues => _updatedValues;

        public RowEditOptions()
        {
            _updatedValues = new Dictionary<string, object>();
        }

        public void AddNewValue(string propertyName, object value)
        {
            if (_updatedValues.ContainsKey(propertyName))
            {
                _updatedValues[propertyName] = value;
            }
            else
            {
                _updatedValues.Add(propertyName, value);
            }
        }
    }
}
