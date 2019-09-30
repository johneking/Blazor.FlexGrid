using Blazor.FlexGrid.Components.Configuration.Builders;
using System;

namespace Blazor.FlexGrid.Components.Configuration.MetaData.Conventions
{
    public abstract class BaseConvention : IConvention
    {
        protected readonly InternalModelBuilder InternalModelBuilder;
        private readonly IGridConfigurationProvider _gridConfigurationProvider;

        protected BaseConvention(IGridConfigurationProvider gridConfigurationProvider)
        {
            _gridConfigurationProvider = gridConfigurationProvider ?? throw new ArgumentNullException(nameof(gridConfigurationProvider));
            InternalModelBuilder = new InternalModelBuilder(gridConfigurationProvider.ConfigurationModel as Model);
        }

        public void Apply(Type type)
        {
            var entityType = _gridConfigurationProvider.FindGridEntityConfigurationByType(type);
            if (entityType is NullEntityType)
            {
                var entityTypeBuilder = InternalModelBuilder
                    .Entity(type);

                Apply(entityTypeBuilder);
            }
            else
            {
                Apply(new InternalEntityTypeBuilder(entityType as EntityType, InternalModelBuilder));
            }
        }

        public abstract void Apply(InternalEntityTypeBuilder internalEntityTypeBuilder);
    }
}
