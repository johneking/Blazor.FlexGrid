namespace Blazor.FlexGrid.Components.Configuration.MetaData
{
	public interface INewItemAnnotations
	{
		bool IsEditable { get; }
		object GetDefault();
	}
}