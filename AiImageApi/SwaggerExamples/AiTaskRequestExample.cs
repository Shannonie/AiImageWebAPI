using AiImage.Core.Models;
using Swashbuckle.AspNetCore.Filters;

namespace AiImage.Api.SwaggerExamples
{
    public class AiTaskRequestExample : IExamplesProvider<AiTaskRequest>
    {
        public AiTaskRequest GetExamples()
        {
            return new AiTaskRequest
            {
                Steps = new List<string> { "stabilityai", "realesrgan",  "removebg"},
                Payload = new ImagePayloadRequest
                {
                    ImageBase64 = "",
                    ImageName = "sample.png",
                    ContentType = "image/png"
                },
                Options = new Dictionary<string, object>
                {
                    { "prompt", "A chic woman sitting in a Paris café, warm sunlight, hussle district, realistic photography." },
                    { "width", 512 },
                    { "height", 512 },
                    { "model", "x4plus"}
                }
            };
        }
    }
}
