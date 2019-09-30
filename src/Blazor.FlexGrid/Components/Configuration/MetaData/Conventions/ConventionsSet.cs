using System;
using System.Collections.Generic;

namespace Blazor.FlexGrid.Components.Configuration.MetaData.Conventions
{
    public class ConventionsSet
    {
        private readonly HashSet<Type> _conventionsRunnedTypes;

        private readonly IGridConfigurationProvider _gridConfigurationProvider;

        public virtual IList<IConvention> Conventions { get; }

        public ConventionsSet(IGridConfigurationProvider gridConfigurationProvider)
        {
            _gridConfigurationProvider = gridConfigurationProvider ?? throw new ArgumentNullException(nameof(gridConfigurationProvider));
            _conventionsRunnedTypes = new HashSet<Type>();

            Conventions = new List<IConvention>
            {
                new MasterDetailConvention(gridConfigurationProvider)
            };
        }

        public virtual void ApplyConventions(Type type)
        {
            if (_conventionsRunnedTypes.Contains(type))
            {
                return;
            }

            foreach (var convention in Conventions)
            {
                convention.Apply(type);
            }

            _conventionsRunnedTypes.Add(type);
        }
    }
}
