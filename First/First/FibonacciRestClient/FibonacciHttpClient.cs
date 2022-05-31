using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace First.FibonacciRestClient
{
    public sealed class FibonacciHttpClient
    {
        private readonly HttpClient _client;
        private readonly HttpClientSettings _settings;

        public FibonacciHttpClient()
        {
            _client = new HttpClient();
            _settings = HttpClientSettings.GetDefaultValue();
        }

        public async Task<string> Get(BigInteger prior, BigInteger current,
            CancellationToken cancellationToken = default)
        {
            var requestUrl = FibonacciClientUrlBuilder.BuildUrl(
                _settings.Uri,
                new NameValueCollection()
                {
                    { "Prior", prior.ToString() }, { "Current", current.ToString() },
                });

            return await InvokeGetAsync(requestUrl, cancellationToken);
        }


        private async Task<string> InvokeGetAsync(string path, CancellationToken cancellationToken)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var httpResponseMessage = await _client.GetAsync(path, cancellationToken);
            return await ProcessHttpResponse(httpResponseMessage);
        }


        private async Task<string> ProcessHttpResponse(HttpResponseMessage httpResponseMessage)
        {
            await CheckResponseForErrors(httpResponseMessage);

            var responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            return responseBody;
        }

        private async Task CheckResponseForErrors(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return;
            }

            var responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

            throw new ApplicationException($"Response Code:{httpResponseMessage}, Full response Body{responseBody}");
        }

        public void Dispose() => _client?.Dispose();
    }
}