using ChainResourceService.Storages.Interfaces;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;

namespace ChainResourceService.Storages
{
    public class WebServiceStorage<T> : IReadOnlyStorage<T>
    {
        #region Private Members

        private readonly string _apiUrl;

        #endregion


        #region Constructors

        public WebServiceStorage(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        #endregion


        #region Public Methods

        public async Task<T> ReadValue()
        {
            using (var handler = new HttpClientHandler { AllowAutoRedirect = false })
            {
                using (var httpClient = new HttpClient(handler))
                {
                    return await GetHttpClientResponse(httpClient);
                }
            }                        
        }

        #endregion


        #region Private Methods

        private async Task<T> GetHttpClientResponse(HttpClient httpClient)
        {
            var response = await httpClient.GetAsync(_apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseBody);
            }
            else if (response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.MovedPermanently)
            {
                string newUrl = response.Headers.Location.AbsoluteUri;
                var newResponse = await httpClient.GetAsync(newUrl);

                if (newResponse.IsSuccessStatusCode)
                {
                    var newResponseBody = await newResponse.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(newResponseBody);
                }
                else
                {
                    throw new HttpRequestException($"Redirected request failed with status code {newResponse.StatusCode}");
                }
            }
            else
            {
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
            }
        }

        #endregion
    }
}
