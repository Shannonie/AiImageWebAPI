using AiImageApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace AiImageApi.SwaggerExamples
{
    public class AiTaskRequestExample : IExamplesProvider<AiTaskRequest>
    {
        public AiTaskRequest GetExamples()
        {
            return new AiTaskRequest
            {
                Steps = new List<string> { "stabilityai", "removebg", "realesrgan" },
                Payload = new ImagePayloadRequest
                {
                    ImageBase64 = "",
                    ImageName = "sample.png",
                    ContentType = "image/png"
                },
                Options = new Dictionary<string, object>
                {
                    { "prompt", "cinematic film still, a cat riding a skateboard through the forest" },
                    { "width", 512 },
                    { "height", 512 },
                    { "model", "x4plus"}
                }
            };
        }
    }
}
