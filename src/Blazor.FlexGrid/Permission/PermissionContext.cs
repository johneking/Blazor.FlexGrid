using Blazor.FlexGrid.Components.Configuration.MetaData;
using System;
using System.Collections.Generic;

namespace Blazor.FlexGrid.Permission
{
    public class PermissionContext
    {
        private readonly ICurrentUserPermission _currentUserPermission;
        private readonly IEntityType _gridEntityConfiguration;
        private readonly Dictionary<string, PermissionAccess> _columnPermissions;

        public bool HasDeleteItemPermission { get; }

        public bool HasCreateItemPermission { get; }

        public PermissionContext(ICurrentUserPermission currentUserPermission, IEntityType gridEntityConfiguration)
        {
            _currentUserPermission = currentUserPermission ?? throw new ArgumentNullException(nameof(currentUserPermission));
            _gridEntityConfiguration = gridEntityConfiguration ?? throw new ArgumentNullException(nameof(gridEntityConfiguration));
            _columnPermissions = new Dictionary<string, PermissionAccess>();

            HasDeleteItemPermission = new GridAnnotations(gridEntityConfiguration)
                .InlineEditOptions.DeletePermissionRestriction(currentUserPermission);

            HasCreateItemPermission = new GridAnnotations(gridEntityConfiguration)
                .CreateItemOptions.CreatePermissionRestriction(currentUserPermission);
        }

        public bool HasCurrentUserReadPermission(string columnName)
        {
            if (_columnPermissions.TryGetValue(columnName, out var permission))
            {
                permission.HasFlag(PermissionAccess.Read);
            }

            return true;
        }

        public bool HasCurrentUserWritePermission(string columnName)
        {
            if (_columnPermissions.TryGetValue(columnName, out var permission))
            {
                permission.HasFlag(PermissionAccess.Write);
            }

            return true;
        }

        public void ResolveColumnPermission(IGridViewColumnAnnotations columnConfig, string columnName)
        {
            var permissionAccess = PermissionAccess.None;
            if (columnConfig is null)
            {
                permissionAccess |= PermissionAccess.Read | PermissionAccess.Write;
            }
            else
            {
                permissionAccess |= columnConfig.ReadPermissionRestrictionFunc(_currentUserPermission)
                   ? PermissionAccess.Read
                   : PermissionAccess.None;

                permissionAccess |= columnConfig.WritePermissionRestrictionFunc(_currentUserPermission)
                   ? PermissionAccess.Write
                   : PermissionAccess.None;
            }

            _columnPermissions.Add(columnName, permissionAccess);
        }
    }
}
