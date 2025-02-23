﻿using System;

namespace Blazor.FlexGrid.Components.Configuration.MetaData
{
    public interface IGridViewAnnotations
    {
        bool IsMasterTable { get; }

        bool OnlyShowExplicitProperties { get; }

        string EmptyItemsMessage { get; }

        InlineEditOptions InlineEditOptions { get; }

        MasterDetailOptions MasterDetailOptions { get; }

        CreateItemOptions CreateItemOptions { get; }

        GridCssClasses CssClasses { get; }

        GlobalGroupingOptions GroupingOptions { get; }

        IMasterDetailRelationship FindRelationshipConfiguration(Type detailType);
    }
}
