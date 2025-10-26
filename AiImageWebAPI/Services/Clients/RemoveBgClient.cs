using AiImageApi.Models;
using AiImageApi.Shared;

namespace AiImageApi.Services.Clients
{
    public class RemoveBgClient :
        ApiClientBase, IImageClient
    {
        private readonly string? _apiKey;

        public RemoveBgClient(
            HttpClient httpClient,
            ILogger<RemoveBgClient> logger,
            IConfiguration config)
            : base(httpClient, logger)
        {
            _apiKey = config["RemoveBg:ApiKey"]
                          ?? Environment.GetEnvironmentVariable("RemoveBg__ApiKey")
                          ?? throw new InvalidOperationException("RemoveBg:ApiKey not set");
        }

        public string ProviderName => "removebg";

        public Task<AiStepResult> ExecuteAsync(
            AiTask<ImagePayload> task,
            CancellationToken ct)
            => SendAsync(task, ct);

        protected override HttpRequestMessage BuildRequest(AiTask<ImagePayload> task)
        {
            try
            {
                MultipartFormDataContent content = BuildMultipartContent(task.Payload, "image_file");
                HttpRequestMessage httpRequest = new HttpRequestMessage(
                    HttpMethod.Post, "https://api.remove.bg/v1.0/removebg")
                {
                    Content = content
                };
                httpRequest.Headers.Add("X-Api-Key", _apiKey);

                return httpRequest;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to build request for RemoveBgClient", ex);
            }
        }

        protected override Task<AiStepResult> ParseResponse(
            HttpResponseMessage response,
            AiTask<ImagePayload> task,
            CancellationToken ct) =>
            ParseBinaryResponse(ProviderName, response, task, ct);
    }
}