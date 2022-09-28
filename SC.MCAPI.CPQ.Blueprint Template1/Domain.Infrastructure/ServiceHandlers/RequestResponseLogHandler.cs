using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Infrastructure.ServiceHandlers
{
    public class RequestResponseLogHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public RequestResponseLogHandler(IHttpContextAccessor httpContextAccessor, ILogger<RequestResponseLogHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            string requestBody = null;
            if (request.Content != null)
            {
                requestBody = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            }

            var result = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Request Response trace - \n" +
                                           "Request Url : {RequestUrl}; \n" +
                                           "Request Body : {RequestBody}; \n" +
                                           "Response Status : {ResponseStatus}; \n" +
                                           "Response Body : {ResponseBody}",
                        $"{request.Method} {request.RequestUri?.AbsoluteUri}",
                        requestBody,
                        $"{(int)response.StatusCode} {response.StatusCode}",
                        result);
                }
                else
                {
                    _logger.LogError("Request Response trace - \n" +
                                     "Request Url : {RequestUrl}; \n" +
                                     "Request Body : {RequestBody}; \n" +
                                     "Scrubbed Request Headers : {ScrubbedRequestHeaders}; \n" +
                                     "Response Status : {ResponseStatus}; \n" +
                                     "Response Body : {ResponseBody}",
                        $"{request.Method} {request.RequestUri?.AbsoluteUri}",
                        requestBody,
                        AddHeadersWithoutAuth(request),
                        response.StatusCode,
                        result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NonBlocker : Request Response Logger Error");
            }

            return response;
        }

        private string AddHeadersWithoutAuth(HttpRequestMessage request)
        {
            var headerString = new StringBuilder();
            foreach (var httpRequestHeader in request.Headers.Where(head =>
                         !head.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)))
            {
                headerString.Append($"{httpRequestHeader.Key} :: {httpRequestHeader.Value.FirstOrDefault()} \n");
            }

            return headerString.ToString();
        }
    }
}
