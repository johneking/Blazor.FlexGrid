using Blazor.FlexGrid.Components.Renderers;
using Blazor.FlexGrid.Filters;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Blazor.FlexGrid.Components.Filters
{
    public class ColumnFilter<TValue> : ComponentBase
    {
        private static readonly Dictionary<string, ColumnFilterState> _stateCache = new Dictionary<string, ColumnFilterState>();

        private const string WrapperCssClass = "filter-wrapper";
        private const string WrapperCssCheckboxClass = "filter-wrapper-checkbox";

        private const FilterOperation StringFilterOperations = FilterOperation.Contains | FilterOperation.EndsWith | FilterOperation.StartsWith | FilterOperation.NotEqual | FilterOperation.Equal;
        private const FilterOperation NumberFilterOperations = FilterOperation.GreaterThan | FilterOperation.GreaterThanOrEqual | FilterOperation.LessThan |
            FilterOperation.LessThanOrEqual | FilterOperation.Equal | FilterOperation.NotEqual;

        delegate bool Parser(string value, out TValue result);
        private static readonly Parser _parser;
        private static readonly FilterOperation _allowedFilterOperations;

        private FilterOperation _selectedFilterOperation;
        private TValue _actualFilterValue;
        private bool _filterDefinitionOpened;
        private bool _filterIsApplied;
        private FilterContext _filterContext;

        [CascadingParameter] FlexGridContext CascadeFlexGridContext { get; set; }

        [Parameter] public string ColumnName { get; set; }

        static ColumnFilter()
        {
            var targetType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

            if (targetType == typeof(string))
            {
                _parser = TryParseString;
                _allowedFilterOperations = StringFilterOperations;
            }
            else if (targetType == typeof(int))
            {
                _parser = TryParseInt;
                _allowedFilterOperations = NumberFilterOperations;
            }
            else if (targetType == typeof(long))
            {
                _parser = TryParseLong;
                _allowedFilterOperations = NumberFilterOperations;
            }
            else if (targetType == typeof(float))
            {
                _parser = TryParseFloat;
                _allowedFilterOperations = NumberFilterOperations;
            }
            else if (targetType == typeof(double))
            {
                _parser = TryParseDouble;
                _allowedFilterOperations = NumberFilterOperations;
            }
            else if (targetType == typeof(decimal))
            {
                _parser = TryParseDecimal;
                _allowedFilterOperations = NumberFilterOperations;
            }
            else if (targetType == typeof(DateTime))
            {
                _parser = TryParseDateTime;
                _allowedFilterOperations = NumberFilterOperations;
            }
            else if (targetType == typeof(DateTimeOffset))
            {
                _parser = TryParseDateTimeOffset;
                _allowedFilterOperations = NumberFilterOperations;
            }
            else if (targetType == typeof(bool))
            {
                _parser = TryParseBool;
            }
            else
            {
                throw new InvalidOperationException($"The type '{targetType}' is not a supported type.");
            }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            LoadStateIfExists();

            var rendererBuilder = new BlazorRendererTreeBuilder(builder);

            rendererBuilder
                .OpenElement(HtmlTagNames.Button, _filterIsApplied ? "action-button action-button-small action-button-filter-active" : "action-button action-button-small")
                .AddAttribute(HtmlJsEvents.OnClick,
                    EventCallback.Factory.Create(this, (MouseEventArgs e) =>
                    {
                        _filterDefinitionOpened = !_filterDefinitionOpened;
                    })
                 )
                .OpenElement(HtmlTagNames.Span)
                .OpenElement(HtmlTagNames.I, "fas fa-filter")
                .CloseElement()
                .CloseElement()
                .CloseElement();


            rendererBuilder.OpenElement(HtmlTagNames.Div,
                _filterDefinitionOpened
                ? _parser != TryParseBool
                        ? $"{WrapperCssClass} {WrapperCssClass}-open"
                        : $"{WrapperCssClass} {WrapperCssClass}-open {WrapperCssCheckboxClass}"
                : $"{WrapperCssClass}");

            if (_parser == TryParseBool)
            {
                BuildRendererTreeForCheckbox(rendererBuilder);
            }
            else
            {
                BuildRendererTreeForFilterOperations(rendererBuilder);
                BuildRendererTreeForInputs(rendererBuilder);
            }

            if (_parser != TryParseBool)
            {
                rendererBuilder.OpenElement(HtmlTagNames.Div, "filter-buttons");
                _ = rendererBuilder.OpenElement(HtmlTagNames.Button, "btn btn-light filter-buttons-clear")
                    .AddAttribute(HtmlJsEvents.OnClick,
                        EventCallback.Factory.Create(this, (MouseEventArgs e) =>
                        {
                            ClearFilter();
                        })
                    )
                    .AddContent("Clear")
                    .CloseElement()
                    .CloseElement();
            }

            rendererBuilder.CloseElement();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _filterContext = CascadeFlexGridContext.FilterContext;
        }

        private void FilterValueChanged(string value)
        {
            _parser(value, out _actualFilterValue);
            AddFilterDefinition();
            _filterDefinitionOpened = false;
        }

        private void FilterBoolValueChanged(bool value)
        {
            _actualFilterValue = (TValue)(object)value;
            _selectedFilterOperation = FilterOperation.Equal;
            AddFilterDefinition();
        }

        private void AddFilterDefinition()
        {
            _filterContext.AddOrUpdateFilterDefinition(new ExpressionFilterDefinition(ColumnName, _selectedFilterOperation, _actualFilterValue));
            _filterIsApplied = true;
            CacheActualState();
        }

        private void ClearFilter()
        {
            _filterContext.RemoveFilter(ColumnName);
            _filterIsApplied = false;
            _filterDefinitionOpened = false;
            _stateCache.Remove(ColumnName);
        }

        private void LoadStateIfExists()
        {
            if (_stateCache.TryGetValue(ColumnName, out var columnFilterState))
            {
                _selectedFilterOperation = columnFilterState.FilterOperation;
                _actualFilterValue = (TValue)columnFilterState.FilterValue;
                _filterIsApplied = true;
            }
        }

        private void CacheActualState()
        {
            var filterState = new ColumnFilterState(_actualFilterValue, _selectedFilterOperation);
            if (!_stateCache.ContainsKey(ColumnName))
            {
                _stateCache.Add(ColumnName, filterState);
            }
            else
            {
                _stateCache[ColumnName] = filterState;
            }
        }

        private void BuildRendererTreeForInputs(BlazorRendererTreeBuilder rendererBuilder)
        {
            if (_parser == TryParseDateTime || _parser == TryParseDateTimeOffset)
            {
                _ = rendererBuilder
                    .OpenElement(HtmlTagNames.Input, "edit-text-field edit-date-field-filter")
                    .AddAttribute(HtmlAttributes.Type, HtmlAttributes.TypeDate)
                    .AddAttribute(HtmlAttributes.Value, FormatDateAsString(_actualFilterValue))
                    .AddAttribute(HtmlJsEvents.OnChange, EventCallback.Factory.Create(this,
                        (ChangeEventArgs e) =>
                        {
                            FilterValueChanged(BindConverterExtensions.ConvertTo(e.Value, string.Empty));
                        }))
                    .CloseElement();

                return;
            }

            rendererBuilder
                .OpenElement(HtmlTagNames.Input, "edit-text-field edit-text-field-filter")
                .AddAttribute(HtmlAttributes.Value, _filterIsApplied ? _actualFilterValue.ToString() : string.Empty)
                .AddAttribute(HtmlJsEvents.OnChange, EventCallback.Factory.Create(this,
                    (ChangeEventArgs e) =>
                    {
                        FilterValueChanged(BindConverterExtensions.ConvertTo(e.Value, string.Empty));
                    }))
                .CloseElement();
        }

        private void BuildRendererTreeForCheckbox(BlazorRendererTreeBuilder rendererBuilder)
        {
            rendererBuilder
                .OpenElement(HtmlTagNames.Label, "switch")
                .OpenElement(HtmlTagNames.Input)
                .AddAttribute(HtmlAttributes.Type, HtmlAttributes.Checkbox)
                .AddAttribute(HtmlAttributes.Value, _actualFilterValue)
                .AddAttribute(HtmlJsEvents.OnChange, EventCallback.Factory.Create(this,
                    (ChangeEventArgs e) =>
                    {
                        FilterBoolValueChanged(BindConverterExtensions.ConvertTo(e.Value, false));
                    }))
                .CloseElement()
                .OpenElement(HtmlTagNames.Span, "slider round")
                .CloseElement()
                .CloseElement();
        }

        private void BuildRendererTreeForFilterOperations(BlazorRendererTreeBuilder rendererBuilder)
        {
            rendererBuilder
                    .OpenElement(HtmlTagNames.Select)
                    .AddAttribute(HtmlJsEvents.OnChange, EventCallback.Factory.Create(this,
                        (ChangeEventArgs e) =>
                        {
                            _selectedFilterOperation = (FilterOperation)BindConverterExtensions.ConvertTo(e.Value, 1);
                            if (_filterIsApplied)
                            {
                                _filterContext.AddOrUpdateFilterDefinition(new ExpressionFilterDefinition(ColumnName, _selectedFilterOperation, _actualFilterValue));
                            }

                        }));

            foreach (var enumValue in Enum.GetValues(typeof(FilterOperation)))
            {
                var filterOperation = (FilterOperation)enumValue;

                if (!_allowedFilterOperations.HasFlag(filterOperation)
                    || filterOperation == FilterOperation.None)
                {
                    continue;
                }

                _selectedFilterOperation = _selectedFilterOperation == FilterOperation.None
                    ? filterOperation
                    : _selectedFilterOperation;

                var enumStringValue = enumValue.ToString();
                rendererBuilder.OpenElement(HtmlTagNames.Option);
                if (enumStringValue == _selectedFilterOperation.ToString())
                {
                    rendererBuilder.AddAttribute(HtmlAttributes.Selected, true);
                }

                rendererBuilder
                    .AddAttribute(HtmlAttributes.Value, (int)enumValue)
                    .AddContent(enumStringValue)
                    .CloseElement();
            }

            rendererBuilder.CloseElement();
        }

        private string FormatDateAsString(TValue value)
        {
            switch (value)
            {
                case DateTime dateTimeValue:
                    return dateTimeValue.ToString(FlexGridContext.DateFormat);
                case DateTimeOffset dateTimeOffsetValue:
                    return dateTimeOffsetValue.ToString(FlexGridContext.DateFormat);
                default:
                    return string.Empty;
            }
        }

        static bool TryParseString(string value, out TValue result)
        {
            result = (TValue)(object)value;

            return true;
        }

        static bool TryParseInt(string value, out TValue result)
        {
            var success = int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedValue);
            if (success)
            {
                result = (TValue)(object)parsedValue;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        static bool TryParseLong(string value, out TValue result)
        {
            var success = long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedValue);
            if (success)
            {
                result = (TValue)(object)parsedValue;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        static bool TryParseFloat(string value, out TValue result)
        {
            var success = float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedValue);
            if (success)
            {
                result = (TValue)(object)parsedValue;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        static bool TryParseDouble(string value, out TValue result)
        {
            var success = double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedValue);
            if (success)
            {
                result = (TValue)(object)parsedValue;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        static bool TryParseDecimal(string value, out TValue result)
        {
            var success = decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedValue);
            if (success)
            {
                result = (TValue)(object)parsedValue;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        static bool TryParseDateTime(string value, out TValue result)
        {
            var success = DateTime.TryParse(value, out var parsedValue);
            if (success)
            {
                result = (TValue)(object)parsedValue;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        static bool TryParseDateTimeOffset(string value, out TValue result)
        {
            var success = DateTimeOffset.TryParse(value, out var parsedValue);
            if (success)
            {
                result = (TValue)(object)parsedValue;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        static bool TryParseBool(string value, out TValue result)
        {
            var success = bool.TryParse(value, out var parsedValue);
            if (success)
            {
                result = (TValue)(object)parsedValue;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
    }

    internal class ColumnFilterState
    {
        public object FilterValue { get; }

        public FilterOperation FilterOperation { get; }

        public ColumnFilterState(object filterValue, FilterOperation filterOperation)
        {
            FilterValue = filterValue;
            FilterOperation = filterOperation;
        }
    }
}
