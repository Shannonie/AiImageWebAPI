using AiImage.Core.Models;

namespace AiImage.Core.Services.Clients
{
    public interface IImageClient
    {
        string ProviderName { get; }
        Task<AiStepResult> ExecuteAsync(AiTask<ImagePayload> task, CancellationToken ct);
    }
}
