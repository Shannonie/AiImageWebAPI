using AiImageApi.Models;
using AiImageApi.Services.Clients;
using System.Diagnostics;

namespace AiImageApi.Services
{
    public class ImageService : IImageService
    {
        private readonly Dictionary<string, IImageClient> _clients;
        private readonly ILogger _logger;
        private readonly string debugDir = Path.Combine("debug_output");

        public ImageService(
            IEnumerable<IImageClient> clients,
            ILogger<ImageService> logger)
        {
            _clients = clients.ToDictionary(c => c.ProviderName.ToLower(), StringComparer.OrdinalIgnoreCase);
            _logger = logger;
        }

        public async Task<AiTaskResponse> ExecuteAsync(
            AiTaskRequest request,
            CancellationToken ct)
        {
            string currentStep = "";
            AiTaskResponse aiTaskResponse = new AiTaskResponse();
            List<AiStepResult> stepResults = new List<AiStepResult>();
            
            Directory.CreateDirectory(debugDir);

            AiTask<ImagePayload> taskPayload = new AiTask<ImagePayload>
            {
                Steps = request.Steps,
                Payload = new ImagePayload
                {
                    ImageBytes = !string.IsNullOrEmpty(request.Payload.ImageBase64) ?
                            Convert.FromBase64String(request.Payload.ImageBase64) : null,
                    ImageName = request.Payload.ImageName,
                    ContentType = request.Payload.ContentType
                },
                Options = request.Options ?? new Dictionary<string, object>()
            };
            Stopwatch totalStopwatch = Stopwatch.StartNew();
            try
            {
                foreach (string step in request.Steps)
                {
                    currentStep = step;
                    if (!_clients.TryGetValue(step.ToLowerInvariant(), out IImageClient? client))
                    {
                        stepResults.Add(new AiStepResult
                        {
                            StepName = step,
                            Status = StepStatus.Failed,
                            Message = $"Provider '{step}' not registered"
                        });
                        continue;
                    }

                    _logger.LogInformation("Executing step {Step}...", step);
                    bool flowControl = await ExecuteStep(step, stepResults, taskPayload, client, ct);
                    if (!flowControl)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                stepResults.Add(new AiStepResult
                {
                    StepName = currentStep,
                    Status = StepStatus.Failed,
                    Message = ex.Message
                });
                _logger.LogError(ex, "Error processing task {stepName}", currentStep);
            }
            finally
            {
                totalStopwatch.Stop();

                aiTaskResponse.Steps = stepResults;
                aiTaskResponse.FinalResult = 
                    stepResults.LastOrDefault(s => s.Status == StepStatus.Success)?.Output;
                aiTaskResponse.Summary = $"Completed " +
                    $"{stepResults.Count(s => s.Status == StepStatus.Success)}" +
                    $"/{request.Steps.Count} steps in " +
                    $"{totalStopwatch.ElapsedMilliseconds} ms";

                _logger.LogInformation("Pipeline complete: {Summary}", aiTaskResponse.Summary);
            }
            
            return aiTaskResponse;
        }

        private async Task<bool> ExecuteStep(
            string step,
            List<AiStepResult> stepResults,
            AiTask<ImagePayload> taskPayload,
            IImageClient client,
            CancellationToken ct)
        {
            DateTime stepStartTime = DateTime.Now;
            Stopwatch stepWatch = Stopwatch.StartNew();
            AiStepResult stepResult = await client.ExecuteAsync(taskPayload, ct);
            stepWatch.Stop();
            stepResult.FinishedAt = stepStartTime + stepWatch.Elapsed;
            bool flowControl = await UpdateStepResult(stepResults, taskPayload, step, stepResult);
            if (!flowControl)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> UpdateStepResult(
            List<AiStepResult> stepResults, 
            AiTask<ImagePayload> taskPayload,
            string step,
            AiStepResult stepResult)
        {
            stepResults.Add(stepResult);
            if (stepResult.Status == StepStatus.Success && stepResult.Output?.ImageBase64 != null)
            {
                taskPayload.Payload = new ImagePayload
                {
                    ImageBytes = Convert.FromBase64String(stepResult.Output.ImageBase64),
                    ImageName = stepResult.Output.ImageName,
                    ContentType = stepResult.Output.ContentType ?? "image/png"
                };

                string debugPath = Path.Combine(debugDir, stepResult.Output.ImageName);
                await File.WriteAllBytesAsync(debugPath, taskPayload.Payload.ImageBytes);
            }
            else
            {
                _logger.LogError("Step {Step} failed: {Error}", step, stepResult.Message);
                return false;
            }

            return true;
        }
    }
}
