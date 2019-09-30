using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.FlexGrid.Filters;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Blazor.FlexGrid.DataSet.Http
{
    public class HttpLazyDataSetLoader<TItem> : ILazyDataSetLoader<TItem> where TItem : class
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpLazyDataSetLoader<TItem>> _logger;

        public HttpLazyDataSetLoader(IHttpClientFactory httpClientFactory, ILogger<HttpLazyDataSetLoader<TItem>> logger)
        {
            _httpClient = httpClientFactory?.Create() ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<LazyLoadingDataSetResult<TItem>> GetTablePageData(
            RequestOptions requestOptions,
            IReadOnlyCollection<IFilterDefinition> filterDefinitions = null)
        {
            var dataUri = requestOptions.BuildUrl();
            try
            {
                if (filterDefinitions != null && filterDefinitions.Any())
                {
                    return _httpClient.PostJsonAsync<LazyLoadingDataSetResult<TItem>>(dataUri, filterDefinitions);
                }
                else
                {
                    return _httpClient.GetJsonAsync<LazyLoadingDataSetResult<TItem>>(dataUri);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during fetching data from [{dataUri}]. Ex: {ex}");

                var emptyResult = new LazyLoadingDataSetResult<TItem>
                {
                    Items = Enumerable.Empty<TItem>().ToList()
                };

                return Task.FromResult(emptyResult);
            }
        }
    }
}
