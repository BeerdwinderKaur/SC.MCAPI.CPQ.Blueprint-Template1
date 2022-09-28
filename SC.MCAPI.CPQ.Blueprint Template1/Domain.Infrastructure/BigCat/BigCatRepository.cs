using Domain.Infrastructure.Interfaces.Auth;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Infrastructure.BigCat
{
    public class BigCatRepository : IBigCatRepository
    {
        private readonly HttpClient _client;
        private readonly IServiceTokenProvider _serviceTokenProvider;

        public BigCatRepository(HttpClient client, IServiceTokenProvider serviceTokenProvider)
        {
            _client = client;
            _serviceTokenProvider = serviceTokenProvider;
        }
        public async Task GetBigCatProduct()
        {
            var productId = "123";
            var url = $"?productId={productId}";

            // Add token to your Request Message header
            var token = await _serviceTokenProvider.GetTokenForClient("MyClientIdFromConfig", "AuthorityFromConfig",
                "resourceIdFromConfig");
            _client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(token);

            var response = await _client.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
        }
    }
}
