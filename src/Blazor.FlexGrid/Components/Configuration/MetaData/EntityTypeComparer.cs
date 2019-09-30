using System;
using System.Collections.Generic;

namespace Blazor.FlexGrid.Components.Configuration.MetaData
{
    public class EntityTypeComparer : IComparer<EntityType>
    {
        public int Compare(EntityType x, EntityType y)
        {
            return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }
    }
}
