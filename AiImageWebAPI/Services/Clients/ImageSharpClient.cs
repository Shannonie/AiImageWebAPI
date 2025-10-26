using AiImageApi.Models;
using AiImageApi.Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace AiImageApi.Services.Clients
{
    public class ImageSharpClient(ILogger<ApiClientBaseUtilities> logger) :
        ApiClientBaseUtilities(logger), IImageClient
    {
        public string ProviderName => "imagesharp";

        public async Task<AiStepResult> ExecuteAsync(
            AiTask<ImagePayload> task,
            CancellationToken ct)
        {
            AiStepResult result = new AiStepResult
            {
                StepName = ProviderName
            };

            try
            {

                if (task.Payload.ImageBytes == null)
                    throw new ArgumentException("Image payload is empty");

                using Image image = Image.Load(task.Payload.ImageBytes);
                image.Mutate(x => x.Grayscale());

                using MemoryStream ms = new MemoryStream();
                await image.SaveAsPngAsync(ms, ct);
                byte[] resultBytes = ms.ToArray();

                result.Status = StepStatus.Success;
                result.Message = "OK";
                result.Output = new ImageResult
                {
                    ImageBase64 = Convert.ToBase64String(resultBytes),
                    ContentType = "image/png",
                    ImageName = SetResultFileName(
                        ProviderName, task.Payload.ContentType, task.Payload.ImageName)
                };
            }
            catch (Exception ex)
            {
                result.Status = StepStatus.Failed;
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
