using AiImage.Core.Models;
using AiImage.Core.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AiImage.Core.Services.Clients
{
    public class OpenAIResult
    {
        public List<Datum>? Data { get; set; }
        public class Datum
        {
            public string? B64Json { get; set; }
        }
    }

    public class OpenAIClient : ApiClientBase, IImageClient
    {
        private readonly string _apiKey;

        public OpenAIClient(
            HttpClient httpClient,
            ILogger<OpenAIClient> logger,
            IConfiguration config)
            : base(httpClient, logger)
        {
            _apiKey = config["OpenAI:ApiKey"]
                      ?? Environment.GetEnvironmentVariable("OpenAI__ApiKey")
                      ?? throw new InvalidOperationException("OpenAI:ApiKey not set");
        }

        public string ProviderName => "openai";

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
                    model = "gpt-image-1",
                    prompt = options["prompt"].ToString(),
                    size = "512x512",
                    n = 1,
                    response_format = "b64_json"
                };

                HttpRequestMessage httpRequest =
                    new HttpRequestMessage(HttpMethod.Post, "v1/images/generations")
                    {
                        Content = BuildJsonContent(payload)
                    };
                httpRequest.Headers.Add("Authorization", $"Bearer {_apiKey}");

                return httpRequest;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to build request for StabilityClient", ex);
            }
        }

        protected override async Task<AiStepResult> ParseResponse(
            HttpResponseMessage response,
            AiTask<ImagePayload> task,
            CancellationToken ct) => await base.ParseJsonBase64Response<OpenAIResult>(
                ProviderName, response, task, ct,
                result => result.Data?.FirstOrDefault()?.B64Json);
    }
}
