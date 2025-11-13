using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AiImage.HybridApp.Services
{
    /// <summary>
    /// Providing common HTTP request functionality.
    /// </summary>
    public abstract class BaseApiService
    {
        protected readonly HttpClient HttpClient;

        protected BaseApiService(HttpClient httpClient)
        {
            HttpClient = httpClient ??
                throw new ArgumentNullException(nameof(httpClient));
        }

        protected async Task<T?> PostAsync<T>(
            string uri,
            object payload,
            string? token = null)
        {
            using HttpRequestMessage request =
                new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = JsonContent.Create(payload)
                };
            AddAuthHeader(request, token);
            HttpResponseMessage response = await HttpClient.SendAsync(request);
            await EnsureSuccessAsync(response);
            return await response.Content.ReadFromJsonAsync<T>();
        }

        protected void AddAuthHeader(HttpRequestMessage request, string? token)
        {
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                string msg = $"API Error: {(int)response.StatusCode} " +
                    $"{response.ReasonPhrase}\nDetails: {body}";
                throw new HttpRequestException(msg);
            }
        }
    }
}
