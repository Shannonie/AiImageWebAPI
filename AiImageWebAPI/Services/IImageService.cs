using AiImageApi.Models;

namespace AiImageApi.Services
{
    public interface IImageService
    {
        Task<AiTaskResponse> ExecuteAsync(AiTaskRequest request, CancellationToken ct);
    }
}