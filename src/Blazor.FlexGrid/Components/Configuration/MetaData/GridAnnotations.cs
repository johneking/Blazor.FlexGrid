using System;

namespace Blazor.FlexGrid.Components.Configuration.MetaData
{
    public class GridAnnotations : IGridViewAnnotations
    {
        private readonly IEntityType _entityTypeMetadata;
        private readonly IAnnotatable _annotations;


        public bool IsMasterTable
        {
            get
            {
                var isMasterAnnotation = _annotations[GridViewAnnotationNames.IsMasterTable];
                if (isMasterAnnotation is NullAnnotationValue)
                {
                    return false;
                }

                return (bool)isMasterAnnotation;
            }
        }


        public MasterDetailOptions MasterDetailOptions
        {
            get
            {
                var masterDetailOptions = _annotations[GridViewAnnotationNames.MasterDetailOptions];
                if (masterDetailOptions is NullAnnotationValue)
                {
                    return NullMasterDetailOptions.Instance;
                }

                return (MasterDetailOptions)masterDetailOptions;
            }
        }

        public GridCssClasses CssClasses
        {
            get
            {
                var cssClasses = _annotations[GridViewAnnotationNames.CssClasses];
                if (cssClasses is NullAnnotationValue)
                {
                    return new DefaultGridCssClasses();
                }

                return cssClasses as GridCssClasses;
            }
        }

        public InlineEditOptions InlineEditOptions
        {
            get
            {
                var inlineEditOptions = _annotations[GridViewAnnotationNames.InlineEditOptions];
                if (inlineEditOptions is NullAnnotationValue)
                {
                    return NullInlineEditOptions.Instance;
                }

                return (InlineEditOptions)inlineEditOptions;
            }
        }

        public CreateItemOptions CreateItemOptions
        {
            get
            {

                var createItemOptions = _annotations[GridViewAnnotationNames.CreateItemOptions];
                if (createItemOptions is NullAnnotationValue)
                {
                    return NullCreateItemOptions.Instance;
                }

                return (CreateItemOptions)createItemOptions;
            }
        }



        public bool OnlyShowExplicitProperties
        {
            get
            {
                var onlyShowExplicitProperties = _annotations[GridViewAnnotationNames.OnlyShowExplicitProperties];
                if (onlyShowExplicitProperties is NullAnnotationValue)
                {
                    return false;
                }

                return (bool)onlyShowExplicitProperties;
            }
        }

        public GlobalGroupingOptions GroupingOptions
        {
            get
            {
                var groupingOptions = _annotations[GridViewAnnotationNames.GroupingOptions];
                if (groupingOptions is NullAnnotationValue)
                {
                    return NullGlobalGroupingOptions.Instance;
                }

                return (GlobalGroupingOptions)groupingOptions;
            }
        }

        public string EmptyItemsMessage
        {
            get
            {
                var emptyItemsMessage = _annotations[GridViewAnnotationNames.EmptyItemsMessage];
                if (emptyItemsMessage is NullAnnotationValue)
                {
                    return "No data to show here ...";
                }

                return emptyItemsMessage.ToString();
            }
        }

        public GridAnnotations(IEntityType entityType)
        {
            _entityTypeMetadata = entityType ?? throw new ArgumentNullException(nameof(entityType));
            _annotations = entityType;
        }

        public IMasterDetailRelationship FindRelationshipConfiguration(Type detailType)
        {
            var masterDetailConnection = _entityTypeMetadata.FindDetailRelationship(detailType);
            if (masterDetailConnection is null)
            {
                throw new InvalidOperationException($"If you want to use Master/Detail functionality, you must configure relationship using method HasDetailRelationship");
            }

            return masterDetailConnection;
        }
    }
}
