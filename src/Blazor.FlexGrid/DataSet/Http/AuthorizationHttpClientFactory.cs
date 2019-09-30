using Blazor.FlexGrid.Permission;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Blazor.FlexGrid.DataSet.Http
{
    public class AuthorizationHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationHttpClientFactory(HttpClient httpClient, IAuthorizationService authorizationService)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        }

        public HttpClient Create()
        {
            if (_httpClient.DefaultRequestHeaders.Authorization is null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authorizationService.UserToken);
            }

            return _httpClient;
        }
    }
}
