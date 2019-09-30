using Blazor.FlexGrid.DataSet.Http.Dto;
using Blazor.FlexGrid.Filters;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Blazor.FlexGrid.DataSet.Http
{
    public class HttpLazyGroupableDataSetLoader<TItem> : ILazyGroupableDataSetLoader<TItem> where TItem : class
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpLazyGroupableDataSetLoader<TItem>> _logger;

        public HttpLazyGroupableDataSetLoader(IHttpClientFactory httpClientFactory, ILogger<HttpLazyGroupableDataSetLoader<TItem>> logger)
        {
            _httpClient = httpClientFactory?.Create() ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LazyLoadingDataSetResult<GroupItem<TItem>>> GetGroupedTablePageData(
            RequestOptions requestOptions,
            IReadOnlyCollection<IFilterDefinition> filterDefinitions = null)
        {
            var dataUri = requestOptions.BuildUrl();
            try
            {
                if (filterDefinitions != null && filterDefinitions.Any())
                {
                    var response = await _httpClient.PostJsonAsync<GroupedItemsDto<TItem>>(dataUri, filterDefinitions);
                    return ConvertGroupedItemsDtoToGroupedResult(response);
                }
                else
                {
                    var response = await _httpClient.GetJsonAsync<GroupedItemsDto<TItem>>(dataUri);
                    return ConvertGroupedItemsDtoToGroupedResult(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during fetching data from [{dataUri}]. Ex: {ex}");
                var emptyResult = new LazyLoadingDataSetResult<GroupItem<TItem>>
                {
                    Items = Enumerable.Empty<GroupItem<TItem>>().ToList()
                };
                return emptyResult;
            }
        }

        private static LazyLoadingDataSetResult<GroupItem<TItem>> ConvertGroupedItemsDtoToGroupedResult(GroupedItemsDto<TItem> groupedItemsDto)
        {
	        var result = new LazyLoadingDataSetResult<GroupItem<TItem>>
	        {
		        Items = new List<GroupItem<TItem>>(),
		        TotalCount = groupedItemsDto.TotalCount
	        };

	        foreach (var pair in groupedItemsDto.Items)
            {
                result.Items.Add(new GroupItem<TItem>(pair.Key, pair.Value));
            }

            return result;
        }
    }
}
