using Blazor.FlexGrid.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.FlexGrid.Components.Filters
{
    public sealed class FilterContext
    {
        private readonly List<IFilterDefinition> _filterDefinitions = new List<IFilterDefinition>();

        public event EventHandler<FilterChangedEventArgs> OnFilterChanged;

        public void AddOrUpdateFilterDefinition(IFilterDefinition filterDefinition)
        {
            var definition = _filterDefinitions.FirstOrDefault(fd => fd.ColumnName == filterDefinition.ColumnName);
            if (definition is null)
            {
                _filterDefinitions.Add(filterDefinition);
            }
            else
            {
                _filterDefinitions.Remove(definition);
                _filterDefinitions.Add(filterDefinition);
            }

            OnFilterChanged?.Invoke(this, new FilterChangedEventArgs(_filterDefinitions));
        }

        public void RemoveFilter(string columnName)
        {
            var definition = _filterDefinitions.FirstOrDefault(fd => fd.ColumnName == columnName);
            if (definition != null)
            {
                _filterDefinitions.Remove(definition);
                OnFilterChanged?.Invoke(this, new FilterChangedEventArgs(_filterDefinitions));
            }
        }
    }
}
