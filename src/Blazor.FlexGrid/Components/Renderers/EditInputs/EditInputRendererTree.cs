using System;

namespace Blazor.FlexGrid.Components.Renderers.EditInputs
{
    public class EditInputRendererTree : AbstractEditInputRenderer
    {
        private readonly AbstractEditInputRenderer _rendererTree;

        public EditInputRendererTree()
        {
            var dateTimeInputRenderer = new DateTimeInputRenderer();
            var textInputRenderer = new TextInputRenderer();
            var numberInputRenderer = new NumberInputType();
            var selectInputRenderer = new SelectInputRenderer();

            numberInputRenderer.SetSuccessor(dateTimeInputRenderer);
            dateTimeInputRenderer.SetSuccessor(selectInputRenderer);
            selectInputRenderer.SetSuccessor(textInputRenderer);

            _rendererTree = numberInputRenderer;
        }

        public override void BuildInputRendererTree(IRendererTreeBuilder rendererTreeBuilder, IActualItemContext<object> actualItemContext, Action<string, object> onChangeAction, string columnName)
            => _rendererTree.BuildInputRendererTree(rendererTreeBuilder, actualItemContext, onChangeAction, columnName);
    }
}
