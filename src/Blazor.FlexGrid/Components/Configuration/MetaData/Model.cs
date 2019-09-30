using System;
using System.Collections.Generic;

namespace Blazor.FlexGrid.Components.Configuration.MetaData
{
    public class Model : Annotatable, IModel
    {
        private readonly SortedDictionary<string, EntityType> _entityTypes;
        private readonly Dictionary<Type, EntityType> _entityTypesMap;

        public Model()
        {
            _entityTypes = new SortedDictionary<string, EntityType>();
            _entityTypesMap = new Dictionary<Type, EntityType>();
        }


        public EntityType AddEntityType(Type clrType)
        {
            var findedEntityType = FindEntityType(clrType);
            if (findedEntityType != null)
            {
                return findedEntityType;
            }

            var entityType = new EntityType(clrType, this);

            _entityTypes.Add(entityType.Name, entityType);
            _entityTypesMap.Add(clrType, entityType);

            return entityType;
        }

        public EntityType FindEntityType(Type clrType)
            => _entityTypesMap.TryGetValue(clrType, out var entityType)
                ? entityType
                : null;

        public EntityType FindEntityType(string name)
            => _entityTypes.TryGetValue(name, out var entityType)
                ? entityType
                : null;

        public IEnumerable<EntityType> GetEntityTypes() => _entityTypes.Values;


        IEntityType IModel.AddEntityType(Type clrType) => AddEntityType(clrType);

        IEntityType IModel.FindEntityType(Type clrType) => FindEntityType(clrType);

        IEnumerable<IEntityType> IModel.GetEntityTypes() => GetEntityTypes();


    }
}
