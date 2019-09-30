using Blazor.FlexGrid.Components.Configuration;
using Blazor.FlexGrid.Components.Configuration.MetaData.Conventions;
using Blazor.FlexGrid.Components.Events;
using Blazor.FlexGrid.Components.Filters;
using Blazor.FlexGrid.Components.Renderers;
using Blazor.FlexGrid.DataAdapters;
using Blazor.FlexGrid.DataSet;
using Blazor.FlexGrid.DataSet.Options;
using Blazor.FlexGrid.Features;
using Blazor.FlexGrid.Filters;
using Blazor.FlexGrid.Permission;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.FlexGrid.Components
{
    public class GridViewInternal : ComponentBase
    {
        private static readonly ITableDataSet EmptyDataSet = new TableDataSet<EmptyDataSetItem>(
            Enumerable.Empty<EmptyDataSetItem>().AsQueryable(), new FilterExpressionTreeBuilder<EmptyDataSetItem>());

        private bool _tableDataSetInitialized;

        private FlexGridContext _fixedFlexGridContext;
        private (ImmutableGridRendererContext ImutableRendererContext, PermissionContext PermissionContext) _gridRenderingContexts;
        protected IEnumerable<IFeature> Features;
        protected ITableDataSet TableDataSet;

        [Inject] IGridRendererTreeBuilder GridRendererTreeBuilder { get; set; }

        [Inject] GridContextFactory RendererContextFactory { get; set; }

        [Inject] IMasterDetailTableDataSetFactory MasterDetailTableDataSetFactory { get; set; }

        [Inject] ConventionsSet ConventionsSet { get; set; }

        [Parameter] public ITableDataAdapter DataAdapter { get; set; }

        [Parameter] public ILazyLoadingOptions LazyLoadingOptions { get; set; } = new LazyLoadingOptions();

        [Parameter] public int PageSize { get; set; }

        [Parameter] public Action<SaveResultArgs> SaveOperationFinished { get; set; }

        [Parameter] public Action<DeleteResultArgs> DeleteOperationFinished { get; set; }

        [Parameter] public Action<ItemCreatedArgs> NewItemCreated { get; set; }

        [Parameter] public Action<ItemClickedArgs> OnItemClicked { get; set; }

        public GridViewInternal()
            : this(DefaultFeatureCollection.AllFeatures)
        {
        }

        protected GridViewInternal(IEnumerable<IFeature> features)
        {
            Features = features ?? throw new ArgumentNullException(nameof(features));
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            var rendererTreeBuilder = new BlazorRendererTreeBuilder(builder);

            RenderFragment<ImmutableGridRendererContext> tableFragment =
                (ImmutableGridRendererContext imutableGridRendererContext) => delegate (RenderTreeBuilder internalBuilder)
            {
                var gridRendererContext = new GridRendererContext(imutableGridRendererContext, new BlazorRendererTreeBuilder(internalBuilder), TableDataSet, _fixedFlexGridContext);
                GridRendererTreeBuilder.BuildRendererTree(gridRendererContext, _gridRenderingContexts.PermissionContext);
            };

            RenderFragment flexGridFragment = delegate (RenderTreeBuilder interalBuilder)
            {
                var internalRendererTreeBuilder = new BlazorRendererTreeBuilder(interalBuilder);
                internalRendererTreeBuilder
                    .OpenComponent(typeof(GridViewTable))
                    .AddAttribute(nameof(ImmutableGridRendererContext), _gridRenderingContexts.ImutableRendererContext)
                    .AddAttribute(BlazorRendererTreeBuilder.ChildContent, tableFragment)
                    .CloseComponent();

                if (_gridRenderingContexts.ImutableRendererContext.CreateItemIsAllowed() &&
                    _fixedFlexGridContext.IsFeatureActive<CreateItemFeature>())
                {
                    internalRendererTreeBuilder
                          .OpenComponent(typeof(CreateItemModal))
                          .AddAttribute(nameof(CreateItemOptions), _gridRenderingContexts.ImutableRendererContext.GridConfiguration.CreateItemOptions)
                          .AddAttribute(nameof(PermissionContext), _gridRenderingContexts.PermissionContext)
                          .AddAttribute(nameof(CreateFormCssClasses), _gridRenderingContexts.ImutableRendererContext.CssClasses.CreateFormCssClasses)
                          .AddAttribute(nameof(NewItemCreated), NewItemCreated)
                          .CloseComponent();
                }
            };

            rendererTreeBuilder
                .OpenComponent(typeof(CascadingValue<FlexGridContext>))
                    .AddAttribute("IsFixed", true)
                    .AddAttribute("Value", _fixedFlexGridContext)
                    .AddAttribute(nameof(BlazorRendererTreeBuilder.ChildContent), flexGridFragment)
                    .CloseComponent();
        }

        protected override async Task OnInitializedAsync()
        {
            _fixedFlexGridContext = CreateFlexGridContext();

            if (DataAdapter != null)
            {
                ConventionsSet.ApplyConventions(DataAdapter.UnderlyingTypeOfItem);
            }

            TableDataSet = GetTableDataSet();
            await TableDataSet.GoToPage(0);

            if (DataAdapter != null)
            {
                _fixedFlexGridContext.FirstPageLoaded = true;
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!_tableDataSetInitialized &&
                DataAdapter != null)
            {
                ConventionsSet.ApplyConventions(DataAdapter.UnderlyingTypeOfItem);
                TableDataSet = GetTableDataSet();
                await TableDataSet.GoToPage(0);

                _fixedFlexGridContext.FirstPageLoaded = true;
            }
        }

        protected virtual FlexGridContext CreateFlexGridContext()
            => new FlexGridContext(new FilterContext(), new FeatureCollection(Features));

        protected ITableDataSet GetTableDataSet()
        {
            var tableDataSet = DataAdapter?.GetTableDataSet(conf =>
            {
                conf.LazyLoadingOptions = LazyLoadingOptions;
                conf.PageableOptions.PageSize = PageSize;
                conf.GridViewEvents = new GridViewEvents
                {
                    SaveOperationFinished = SaveOperationFinished,
                    DeleteOperationFinished = DeleteOperationFinished,
                    NewItemCreated = NewItemCreated,
                    OnItemClicked = OnItemClicked
                };
            });

            if (tableDataSet is null)
            {
                tableDataSet = EmptyDataSet;
            }
            else
            {
                tableDataSet = MasterDetailTableDataSetFactory.ConvertToMasterTableIfIsRequired(tableDataSet);
                if (_fixedFlexGridContext.IsFeatureActive<FilteringFeature>())
                {
                    _fixedFlexGridContext.FilterContext.OnFilterChanged += FilterChanged;
                }

                _tableDataSetInitialized = true;
            }

            _gridRenderingContexts = RendererContextFactory.CreateContexts(tableDataSet);
            if (_fixedFlexGridContext.IsFeatureActive<GroupingFeature>())
            {
                tableDataSet.GroupingOptions.SetConfiguration(
                    _gridRenderingContexts.ImutableRendererContext.GridConfiguration.GroupingOptions,
                    _gridRenderingContexts.ImutableRendererContext.GridItemProperties);
            }

            if (tableDataSet is IMasterTableDataSet)
            {
                _fixedFlexGridContext.Features.Set<IMasterTableFeature>(new MasterTableFeature(DataAdapter));
            }

            return tableDataSet;
        }

        private void FilterChanged(object sender, FilterChangedEventArgs e)
        {
            TableDataSet.ApplyFilters(e.Filters);
            _fixedFlexGridContext.RequestRerenderTableRowsNotification?.Invoke();
        }
    }
}
