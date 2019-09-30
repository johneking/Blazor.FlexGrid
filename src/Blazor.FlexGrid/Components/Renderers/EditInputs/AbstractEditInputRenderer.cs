using System;

namespace Blazor.FlexGrid.Components.Renderers.EditInputs
{
    public abstract class AbstractEditInputRenderer
    {
        protected AbstractEditInputRenderer Successor;

        public void SetSuccessor(AbstractEditInputRenderer editInputRenderer)
            => Successor = editInputRenderer ?? throw new ArgumentNullException(nameof(editInputRenderer));

        public abstract void BuildInputRendererTree(IRendererTreeBuilder rendererTreeBuilder, IActualItemContext<object> actualItemContext, Action<string, object> onChangeAction, string columnName);
    }
}
