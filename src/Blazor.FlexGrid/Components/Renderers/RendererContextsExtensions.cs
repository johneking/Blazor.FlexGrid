﻿namespace Blazor.FlexGrid.Components.Renderers
{
    public static class RendererContextsExtensions
    {
        public static bool ShowExplicitDetailTables(this GridRendererContext rendererContext)
            => rendererContext.GridConfiguration.MasterDetailOptions.OnlyShowExplicitDetailTables;

        public static bool CreateItemIsAllowed(this GridRendererContext rendererContext)
            => rendererContext.GridConfiguration.CreateItemOptions.IsCreateItemAllowed;

        public static bool CreateItemIsAllowed(this ImmutableGridRendererContext rendererContext)
            => rendererContext.GridConfiguration.CreateItemOptions.IsCreateItemAllowed;

        public static bool InlineEditItemIsAllowed(this GridRendererContext rendererContext)
            => rendererContext.GridConfiguration.InlineEditOptions.InlineEditIsAllowed;

        public static bool InlineEditItemIsAllowed(this ImmutableGridRendererContext rendererContext)
            => rendererContext.GridConfiguration.InlineEditOptions.InlineEditIsAllowed;
    }
}
