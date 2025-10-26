using Swashbuckle.AspNetCore.Annotations;

namespace AiImageApi.Models
{
    public class AiTaskRequest
    {
        [SwaggerSchema("List of processing steps (e.g, stability, openai, removebg, real-esrgan, imagesharp")]
        public required List<string> Steps { get; set; } = [];

        [SwaggerSchema("Image payload with base64 image and metadata.")]
        public required ImagePayloadRequest Payload { get; set; }

        [SwaggerSchema("Optional parameters for each client (e.g., prompt, width, height, model).")]
        public Dictionary<string, object> Options { get; set; } = new();
    }

    public class ImagePayloadRequest
    {
        [SwaggerSchema("Base64-encoded image data.")]
        public required string ImageBase64 { get; set; }

        [SwaggerSchema("Image file name.")]
        public string ImageName { get; set; } = "input.png";

        [SwaggerSchema("MIME type of the image (e.g., image/png, image/jpeg).")]
        public string ContentType { get; set; } = "image/png";
    }
}
