using AiImage.Core.Models;

namespace AiImage.HybridApp.Services
{
    public class ApiService : BaseApiService, IApiService
    {
        public ApiService(HttpClient httpClient) : base(httpClient) { }

        public async Task<AiTaskResponse?> ProcessImageAsync(
            AiTaskRequest request,
            string token)
        {
            return await PostAsync<AiTaskResponse>("api/Image/process", request, token);
        }
    }
}
