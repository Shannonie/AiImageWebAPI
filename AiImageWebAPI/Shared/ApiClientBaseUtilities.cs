namespace AiImageApi.Shared
{
    public abstract class ApiClientBaseUtilities
    {
        private readonly ILogger _logger;

        protected ApiClientBaseUtilities(ILogger<ApiClientBaseUtilities> logger)
        {
            _logger = logger;
        }

        protected async Task CheckResponseSuccess(
            string stepName,
            HttpResponseMessage response,
            CancellationToken ct)
        {
            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync(ct);
                string msg = error.Substring(0, Math.Min(300, error.Length));
                string errMsg = $"{stepName} API error {response.StatusCode}: {error}";
                _logger.LogError(errMsg);
                throw new HttpRequestException(errMsg);
            }
            ///ReadAndLogRawResponse(stepName, response, ct);
        }

        protected async Task<string> ReadAndLogRawResponse(
            string stepName,
            HttpResponseMessage response,
            CancellationToken ct)
        {
            string? contentType = response.Content.Headers.ContentType?.MediaType;
            
            string raw = await response.Content.ReadAsStringAsync(ct);
            
            string trimmed = raw.TrimStart('\uFEFF');
            
            string preview = trimmed.Length > 500 ? trimmed[..500] + "..." : trimmed;
            _logger.LogInformation("Raw response from {Step}({Type}):\n{Preview}",
                stepName, contentType, preview);

            return trimmed;
        }

        protected string? SetResultFileName(string stepName, string contentType, string? oriName)
        {
            string prefix = stepName.ToLower() switch
            {
                "realesrgan" => "upscale",
                "removebg" => "nobg",
                "imagesharp" => "filter",
                "stabilityai" => "gen",
                "openai" => "gen",
                _ => "result"
            };

            string extension = contentType switch
            {
                "image/jpeg" or "image/jpg" => "jpg",
                "image/png" => "png",
                _ => "png"
            };

            string name = Path.GetFileNameWithoutExtension(oriName ?? "image");
            return $"{prefix}_{name}.{extension}";
        }
    }
}