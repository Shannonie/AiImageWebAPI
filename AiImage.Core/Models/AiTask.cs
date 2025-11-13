namespace AiImage.Core.Models
{
    public class AiTask<TPayload>
    {
        public List<string> Steps { get; set; } = new();
        public TPayload Payload { get; set; } = default!;
        public Dictionary<string, object> Options { get; set; } = new();
    }

    public class ImagePayload
    {
        public byte[]? ImageBytes { get; set; }
        public string? ImageName { get; set; }
        public string ContentType { get; set; } = "image/png";
    }
}
