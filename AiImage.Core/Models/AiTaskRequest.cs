namespace AiImage.Core.Models
{
    public class AiTaskRequest
    {
        public required List<string> Steps { get; set; } = [];

        public required ImagePayloadRequest Payload { get; set; }

        public Dictionary<string, object> Options { get; set; } = new();
    }

    public class ImagePayloadRequest
    {
        public required string ImageBase64 { get; set; }

        public string ImageName { get; set; } = "input.png";

        public string ContentType { get; set; } = "image/png";
    }
}
