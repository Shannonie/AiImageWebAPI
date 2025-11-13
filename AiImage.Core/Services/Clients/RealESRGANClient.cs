using AiImage.Core.Models;
using AiImage.Core.Shared;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AiImage.Core.Services.Clients
{
    public class RealESRGANClient(HttpClient httpClient, ILogger<RealESRGANClient> logger) :
        ApiClientBase(httpClient, logger), IImageClient
    {
        public string ProviderName => "realesrgan";

        public Task<AiStepResult> ExecuteAsync(
            AiTask<ImagePayload> task,
            CancellationToken ct)
            => SendAsync(task, ct);

        protected override HttpRequestMessage BuildRequest(AiTask<ImagePayload> task)
        {
            try
            {
                MultipartFormDataContent content = BuildMultipartContent(task.Payload, "file");
                string url = BuildRequestUrl(task.Options);
                return new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to build request for {ProviderName}", ex);
            }
        }

        protected override async Task<AiStepResult> ParseResponse(
            HttpResponseMessage response,
            AiTask<ImagePayload> task,
            CancellationToken ct) => await ParseBinaryResponse(ProviderName, response, task, ct);

        private string BuildRequestUrl(Dictionary<string, object> options)
        {
            string model = options.ContainsKey("model") ? options["model"].ToString()! : "x4plus";
            bool half = options.ContainsKey("half") && ((JsonElement)options["half"]).GetBoolean();
            int tile = options.ContainsKey("tile") ? ((JsonElement)options["tile"]).GetInt32() : 0;
            int tilePad = options.ContainsKey("tile_pad") ? ((JsonElement)options["tile_pad"]).GetInt32() : 10;
            float denoise = options.ContainsKey("denoise") ? ((JsonElement)options["denoise"]).GetSingle() : 1f;
            int scale = options.ContainsKey("scale_size") ? ((JsonElement)options["scale_size"]).GetInt32() : 1;
            string query = $"?model={model}&half={half}&tile={tile}&tile_pad={tilePad}&scale={scale}";
            if (model.ToLower() == "general-x4v3")
                query += $"&denoise={denoise}";

            return $"http://localhost:8001/upscale{query}";
        }
    }
}
