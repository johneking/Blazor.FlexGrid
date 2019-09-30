namespace Blazor.FlexGrid.Components.Renderers
{
    public interface IActualItemContext<out TItem> where TItem : class
    {
	    TItem ActualItem { get; }

        object GetActualItemColumnValue(string columnName);

        void SetActualItemColumnValue(string columnName, object value);
    }
}
