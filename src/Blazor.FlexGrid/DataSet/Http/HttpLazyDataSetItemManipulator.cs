using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blazor.FlexGrid.Components.Configuration.ValueFormatters;
using Blazor.FlexGrid.DataSet.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace Blazor.FlexGrid.DataSet.Http
{
    public class HttpLazyDataSetItemManipulator<TItem> : ILazyDataSetItemManipulator<TItem> where TItem : class
    {
        private const string DeleteUrlPattern = @"\/{\s*(?<prop>\w+)\s*(,\s*(?<prop>\w+)\s*)*}";

        private readonly HttpClient _httpClient;
        private readonly ITypePropertyAccessorCache _propertyValueAccessorCache;
        private readonly ILogger<HttpLazyDataSetItemManipulator<TItem>> _logger;

        public HttpLazyDataSetItemManipulator(
            IHttpClientFactory httpClientFactory,
            ITypePropertyAccessorCache propertyValueAccessorCache,
            ILogger<HttpLazyDataSetItemManipulator<TItem>> logger)
        {
            _httpClient = httpClientFactory?.Create() ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _propertyValueAccessorCache = propertyValueAccessorCache ?? throw new ArgumentNullException(nameof(propertyValueAccessorCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TItem> SaveItem(TItem item, ILazyLoadingOptions lazyLoadingOptions)
        {
            if (string.IsNullOrWhiteSpace(lazyLoadingOptions.PutDataUri))
            {
                throw new ArgumentNullException($"When you are using {nameof(LazyTableDataSet<TItem>)} you must specify url for saving updated item data. " +
                    $"If you are using {nameof(LazyTableDataSet<TItem>)} as detail GridView you must configure url by calling method HasUpdateUrl");
            }

            try
            {
                var response = await _httpClient.PutJsonAsync<TItem>(lazyLoadingOptions.PutDataUri, item);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during saving data for [{lazyLoadingOptions.PutDataUri}]. Ex: {ex}");

                return null;
            }
        }

        public async Task<TItem> DeleteItem(TItem item, ILazyLoadingOptions lazyLoadingOptions)
        {
            if (string.IsNullOrWhiteSpace(lazyLoadingOptions.DeleteUri))
            {
                throw new ArgumentNullException($"When you are using {nameof(LazyTableDataSet<TItem>)} you must specify url for deleteing item. " +
                    $"If you are using {nameof(LazyTableDataSet<TItem>)} as detail GridView you must configure url by calling method HasDeleteUrl");
            }

            var match = Regex.Match(lazyLoadingOptions.DeleteUri, DeleteUrlPattern);
            if (!match.Success)
            {
                throw new InvalidOperationException("The DeleteUri must contains {} with name of property with key value.");
            }

            var realDeleteUri = lazyLoadingOptions.DeleteUri;

            try
            {
                var keyPropertyNames = match.Groups["prop"].Captures;
                var propertyValueAccessor = _propertyValueAccessorCache.GetPropertyAccesor(typeof(TItem));
                var query = new QueryBuilder();
                foreach (Capture keyPropertyName in keyPropertyNames)
                {
                    var keyValue = propertyValueAccessor.GetValue(item, keyPropertyName.Value);
                    query.Add(keyPropertyName.Value, keyValue.ToString());
                }
                realDeleteUri = Regex.Replace(lazyLoadingOptions.DeleteUri, DeleteUrlPattern, query.ToString());

                var response = await _httpClient.DeleteAsync(realDeleteUri);
                if (response.IsSuccessStatusCode)
                {
                    return item;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during deleting item for uri [{realDeleteUri}]. Ex: {ex}");

                return null;
            }
        }
    }
}
