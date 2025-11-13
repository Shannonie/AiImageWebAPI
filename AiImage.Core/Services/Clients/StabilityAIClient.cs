using AiImage.Core.Models;
using AiImage.Core.Shared;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AiImage.Core.Services.Clients
{
    public class StabilityResult
    {
        public List<Artifact>? Artifacts { get; set; }
        public class Artifact
        {
            public string? Base64 { get; set; }
        }
    }

    public class StabilityAIClient(HttpClient httpClient, ILogger<StabilityAIClient> logger) :
        ApiClientBase(httpClient, logger), IImageClient
    {
        public string ProviderName => "stabilityai";

        public Task<AiStepResult> ExecuteAsync(
            AiTask<ImagePayload> task,
            CancellationToken ct)
            => SendAsync(task, ct);

        protected override HttpRequestMessage BuildRequest(AiTask<ImagePayload> task)
        {
            try
            {
                Dictionary<string, object> options = task.Options;

                if (!options.ContainsKey("prompt") || string.IsNullOrEmpty(options["prompt"].ToString()))
                    throw new ArgumentException("ImagePayload.Prompt cannot be null or empty");

                var payload = new
                {
                    prompt = options["prompt"].ToString(),
                    width = options.ContainsKey("width") ? ((JsonElement)options["width"]).GetInt32() : 512,
                    height = options.ContainsKey("height") ? ((JsonElement)options["height"]).GetInt32() : 512,
                    steps = options.ContainsKey("steps") ? ((JsonElement)options["steps"]).GetInt32() : 30
                };

                HttpRequestMessage httpRequest =
                    new HttpRequestMessage(HttpMethod.Post, "http://localhost:8000/generate")
                    {
                        Content = BuildJsonContent(payload)
                    };
                ///httpRequest.Headers.Add("Authorization", $"Bearer {_apiKey}");

                return httpRequest;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to build request for StabilityClient", ex);
            }
        }

        protected override Task<AiStepResult> ParseResponse(
            HttpResponseMessage response,
            AiTask<ImagePayload> task,
            CancellationToken ct) =>
            ParseBinaryResponse(ProviderName, response, task, ct);
    }
}