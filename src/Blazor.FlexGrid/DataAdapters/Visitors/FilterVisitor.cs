using Blazor.FlexGrid.Components.Configuration.MetaData;
using Blazor.FlexGrid.Components.Configuration.ValueFormatters;
using Blazor.FlexGrid.DataSet.Options;
using System;
using System.Linq.Expressions;

namespace Blazor.FlexGrid.DataAdapters.Visitors
{
    public class FilterVisitor<TItem> : IDataTableAdapterVisitor where TItem : class
    {
        private readonly IMasterDetailRelationship _masterDetailRelationship;
        private readonly IMasterDetailRowArguments _masterDetailRowArguments;
        private readonly ITypePropertyAccessorCache _propertyValueAccessorCache;

        public FilterVisitor(
            IMasterDetailRelationship masterDetailRelationship,
            IMasterDetailRowArguments masterDetailRowArguments,
            ITypePropertyAccessorCache propertyValueAccessorCache)
        {
            _masterDetailRelationship = masterDetailRelationship ?? throw new ArgumentNullException(nameof(masterDetailRelationship));
            _masterDetailRowArguments = masterDetailRowArguments ?? throw new ArgumentNullException(nameof(masterDetailRowArguments));
            _propertyValueAccessorCache = propertyValueAccessorCache ?? throw new ArgumentNullException(nameof(propertyValueAccessorCache));
        }

        public void Visit(ITableDataAdapter tableDataAdapter)
        {
            if (tableDataAdapter is CollectionTableDataAdapter<TItem> collectionTableDataAdapter)
            {
                var selectedItemType = _masterDetailRowArguments.SelectedItem.GetType();
                var detailAdapterItemType = _masterDetailRowArguments.DataAdapter.UnderlyingTypeOfItem;

                var constantValue = _propertyValueAccessorCache
                    .GetPropertyAccesor(selectedItemType)
                    .GetValue(_masterDetailRowArguments.SelectedItem, _masterDetailRelationship.MasterDetailConnection.MasterPropertyName);

                var parameter = Expression.Parameter(detailAdapterItemType, "x");
                var member = Expression.Property(parameter, _masterDetailRelationship.MasterDetailConnection.ForeignPropertyName);

                //If the type of the member expression is a nullable,
                //the call to Expression.Equal will fail
                Expression constant;
                if (member.Type.IsGenericType && member.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    constant = Expression.Constant(constantValue, member.Type);
                else
                    constant = Expression.Constant(constantValue);

                var body = Expression.Equal(member, constant);

                collectionTableDataAdapter.Filter = Expression.Lambda<Func<TItem, bool>>(body, parameter);
            }
        }
    }
}
