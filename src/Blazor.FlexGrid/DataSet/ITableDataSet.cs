using Blazor.FlexGrid.Components.Events;

namespace Blazor.FlexGrid.DataSet
{

    /// <summary>
    /// Represents a collection of Items with paging, sorting and inline edit
    /// </summary>
    public interface ITableDataSet : IPageableTableDataSet, ISortableTableDataSet, ISelectableDataSet, IRowEditableDataSet, IFilterableDataSet, IGroupableTableDataSet
    {
        GridViewEvents GridViewEvents { get; }
    }
}
