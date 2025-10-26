using AiImageApi.Models;

namespace AiImageApi.Services.Clients
{
    public interface IImageClient
    {
        string ProviderName { get; }
        Task<AiStepResult> ExecuteAsync(AiTask<ImagePayload> task, CancellationToken ct);
    }
}
