using System.Net.Http.Json;

namespace RaceDay.MVC.Services
{
    public class RaceDayApiService
    {
        private readonly HttpClient _httpClient;

        public RaceDayApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("RaceDayAPI");
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            return await _httpClient.GetFromJsonAsync<T>(endpoint);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(
            string endpoint,
            TRequest data)
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<HttpResponseMessage> PostForResponseAsync<TRequest>(
            string endpoint,
            TRequest data)
        {
            return await _httpClient.PostAsJsonAsync(endpoint, data);
        }
    }
}