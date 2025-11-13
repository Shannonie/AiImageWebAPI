using AiImage.Core.Models;

namespace AiImage.HybridApp.Services
{
    public interface IApiService
    {
        Task<AiTaskResponse?> ProcessImageAsync(AiTaskRequest request, string token);
    }
}
