﻿using Blazor.FlexGrid.Components.Filters;
using Blazor.FlexGrid.Features;
using System;

namespace Blazor.FlexGrid.Components
{
    public class FlexGridContext
    {
        public static readonly string DateFormat = "yyyy-MM-dd"; // Compatible with HTML date inputs

        public FilterContext FilterContext { get; }

        public Action RequestRerenderTableRowsNotification { get; private set; }

        public IFeatureCollection Features { get; }

        public bool FirstPageLoaded { get; set; }

        public FlexGridContext(FilterContext filterContext, IFeatureCollection features)
        {
            FilterContext = filterContext ?? throw new ArgumentNullException(nameof(filterContext));
            Features = features ?? throw new ArgumentNullException(nameof(features));
        }

        public void SetRequestRendererNotification(Action requestRendererNotification)
        {
            if (requestRendererNotification is null)
            {
                return;
            }

            RequestRerenderTableRowsNotification = requestRendererNotification;
        }

        public bool IsFeatureActive<TFeature>() where TFeature : IFeature
            => Features.Contains<TFeature>();
    }
}
