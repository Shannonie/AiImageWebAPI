using AiImageApi.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AiImageApi.Shared
{
    public abstract class ApiClientBase : ApiClientBaseUtilities
    {
        private readonly HttpClient _httpClient;

        protected ApiClientBase(
            HttpClient httpClient,
            ILogger<ApiClientBaseUtilities> logger
            ) : base(logger)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = Timeout.InfiniteTimeSpan;
        }

        protected abstract HttpRequestMessage BuildRequest(AiTask<ImagePayload> task);

        protected abstract Task<AiStepResult> ParseResponse(
            HttpResponseMessage response,
            AiTask<ImagePayload> task,
            CancellationToken ct);

        #region Request Builders
        protected MultipartFormDataContent BuildMultipartContent(
            ImagePayload payload,
            string formField = "image_file")
        {
            if (payload?.ImageBytes == null || payload.ImageBytes.Length == 0)
                throw new ArgumentException("ImagePayload.ImageBytes cannot be null or empty");

            ByteArrayContent byteContent = new ByteArrayContent(payload.ImageBytes);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue(payload.ContentType ?? "image/png");
            
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(byteContent, formField, payload.ImageName ?? "image.png");

            return content;
        }

        protected StringContent BuildJsonContent(object content) =>
            new(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
        #endregion

        #region Response Parsers
        protected async Task<AiStepResult> ParseBinaryResponse(
            string stepName,
            HttpResponseMessage response,
            AiTask<ImagePayload> task,
            CancellationToken ct)
        {
            AiStepResult result = new AiStepResult
            {
                StepName = stepName,
                Status = StepStatus.Success,
                Message = "OK"
            };

            try
            {
                await CheckResponseSuccess(stepName, response, ct);

                byte[] bytes = await response.Content.ReadAsByteArrayAsync(ct);
                string contentType = response.Content.Headers.ContentType?.MediaType ?? "image/png";
                result.Output = new ImageResult
                {
                    ImageBase64 = Convert.ToBase64String(bytes),
                    ContentType = contentType,
                    ImageName = SetResultFileName(contentType, task.Payload.ImageName, stepName)
                };
            }
            catch (Exception ex)
            {
                result.Status = StepStatus.Failed;
                result.Message = $"Failed to parse response: {ex.Message}";
            }

            return result;
        }

        protected async Task<AiStepResult> ParseJsonBase64Response<TJson>(
            string stepName,
            HttpResponseMessage response,
            AiTask<ImagePayload> task,
            CancellationToken ct,
            Func<TJson, string?> extractBase64)
        {
            AiStepResult result = new AiStepResult
            {
                StepName = stepName,
                Status = StepStatus.Success,
                Message = "OK"
            };

            try
            {
                await CheckResponseSuccess(stepName, response, ct);

                string json = await response.Content.ReadAsStringAsync(ct);
                json = json.TrimStart('\uFEFF'); // remove BOM if any
                TJson resultJson = JsonSerializer.Deserialize<TJson>(json)
                    ?? throw new InvalidOperationException("Failed to parse JSON response");

                string? base64 = extractBase64(resultJson);
                if (string.IsNullOrEmpty(base64))
                    throw new InvalidOperationException("No image data returned");
                
                result.Output = new ImageResult
                {
                    ImageBase64 = base64,
                    ContentType = "image/png",
                    ImageName = SetResultFileName("image/png", task.Payload.ImageName, stepName)
                };
            }
            catch (Exception ex)
            {
                result.Status = StepStatus.Failed;
                result.Message = $"Failed to parse JSON base64 response: {ex.Message}";
            }

            return result;
        }

        #endregion

        public async Task<AiStepResult> SendAsync(
            AiTask<ImagePayload> task,
            CancellationToken ct)
        {
            using HttpRequestMessage httpRequest = BuildRequest(task);
            using HttpResponseMessage response = await _httpClient.SendAsync(httpRequest, ct);

            return await ParseResponse(response, task, ct);
        }
    }
}
