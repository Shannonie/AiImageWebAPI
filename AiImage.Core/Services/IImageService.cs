using AiImage.Core.Models;

namespace AiImage.Core.Services
{
    public interface IImageService
    {
        Task<AiTaskResponse> ExecuteAsync(AiTaskRequest request, CancellationToken ct);
    }
}