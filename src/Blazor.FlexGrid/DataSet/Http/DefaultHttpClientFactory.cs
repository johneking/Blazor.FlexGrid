using System;
using System.Net.Http;

namespace Blazor.FlexGrid.DataSet.Http
{
    public class DefaultHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient _httpClient;

        public DefaultHttpClientFactory(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public HttpClient Create()
            => _httpClient;
    }
}
