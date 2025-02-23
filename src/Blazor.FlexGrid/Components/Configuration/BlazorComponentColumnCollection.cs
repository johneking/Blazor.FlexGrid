﻿using Blazor.FlexGrid.Components.Configuration.Builders;
using Blazor.FlexGrid.Components.Configuration.MetaData;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq.Expressions;

namespace Blazor.FlexGrid.Components.Configuration
{
    public class BlazorComponentColumnCollection<TItem> : ISpecialColumnFragmentsCollection<TItem>
    {
        private readonly InternalModelBuilder _internalModelBuilder;
        private readonly IGridConfigurationProvider _gridConfigurationProvider;

        public BlazorComponentColumnCollection(IGridConfigurationProvider gridConfigurationProvider)
        {
            _gridConfigurationProvider = gridConfigurationProvider ?? throw new ArgumentNullException(nameof(gridConfigurationProvider));
            _internalModelBuilder = new InternalModelBuilder(gridConfigurationProvider.ConfigurationModel as Model);
        }

        public ISpecialColumnFragmentsCollection<TItem> AddColumnValueRenderFunction<TColumn>(
            Expression<Func<TItem, TColumn>> columnExpression,
            RenderFragment<TItem> renderFragment)
        {
            GetPropertyBuilder(columnExpression)
                ?.HasBlazorComponentValue(renderFragment);

            return this;
        }

        public ISpecialColumnFragmentsCollection<TItem> AddColumnEditValueRenderer<TColumn>(
            Expression<Func<TItem, TColumn>> columnExpression,
            Func<EditColumnContext, RenderFragment<TItem>> renderFragmentBuilder)
        {
            GetPropertyBuilder(columnExpression)
                ?.HasBlazorEditComponent(renderFragmentBuilder);

            return this;
        }

        private InternalPropertyBuilder GetPropertyBuilder<TColumn>(Expression<Func<TItem, TColumn>> columnExpression)
        {
            var entityType = _gridConfigurationProvider.FindGridEntityConfigurationByType(typeof(TItem));
            if (entityType is NullEntityType)
            {
                return _internalModelBuilder
                    .Entity(typeof(TItem))
                    .Property(columnExpression.GetPropertyAccess());
            }

            var columnName = columnExpression.GetPropertyAccess().Name;
            var configurationProperty = entityType.FindProperty(columnName);
            if (configurationProperty is Property property)
            {
                return new InternalPropertyBuilder(property, _internalModelBuilder);
            }

            return null;
        }
    }
}
