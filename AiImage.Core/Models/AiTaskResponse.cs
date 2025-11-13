namespace AiImage.Core.Models
{
    public enum StepStatus
    {
        Pending,
        Success,
        Failed,
        Skipped
    }

    public class AiTaskResponse
    {
        public string JobId { get; set; } = Guid.NewGuid().ToString();

        public List<AiStepResult> Steps { get; set; } = new();

        public ImageResult? FinalResult { get; set; }

        public string? Summary { get; set; }
    }

    public class AiStepResult
    {
        public string StepName { get; set; } = string.Empty;

        public StepStatus Status { get; set; } = StepStatus.Pending;

        public string? Message { get; set; }

        public ImageResult? Output { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        public DateTime? FinishedAt { get; set; }
    }

    public class ImageResult
    {
        public string ImageBase64 { get; set; } = string.Empty;

        public string ImageName { get; set; } = string.Empty;

        public string ContentType { get; set; } = "image/png";
    }
}
