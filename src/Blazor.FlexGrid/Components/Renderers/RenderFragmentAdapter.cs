using Microsoft.AspNetCore.Components;
using System;

namespace Blazor.FlexGrid.Components.Renderers
{
    public interface IRenderFragmentAdapter
    {
        RenderFragment GetColumnFragment(object item);
    }

    public interface IRenderFragmentAdapter<in TItem> : IRenderFragmentAdapter
    {
        RenderFragment GetColumnFragment(TItem item);
    }

    public class RenderFragmentAdapter<TItem> : IRenderFragmentAdapter<TItem>
    {
        private readonly RenderFragment<TItem> _renderFragment;


        public RenderFragmentAdapter(RenderFragment<TItem> renderFragment)
        {
            _renderFragment = renderFragment ?? throw new ArgumentNullException(nameof(renderFragment));
        }

        public RenderFragment GetColumnFragment(TItem item)
            => _renderFragment(item);

        public RenderFragment GetColumnFragment(object item)
            => GetColumnFragment((TItem)item);
    }
}
