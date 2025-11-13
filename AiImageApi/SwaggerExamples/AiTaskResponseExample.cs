using AiImage.Core.Models;
using Swashbuckle.AspNetCore.Filters;

namespace AiImage.Api.SwaggerExamples
{
    public class AiTaskResponseExample : IExamplesProvider<AiTaskResponse>
    {
        public AiTaskResponse GetExamples()
        {
            return new AiTaskResponse
            {
                JobId = "job-12345",
                Steps = new List<AiStepResult>
                {
                    new AiStepResult
                    {
                        StepName = "stabilityai",
                        Status = StepStatus.Success,
                        Message = "Generate image successfully",
                        Output = new ImageResult
                        {
                            ImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAUA...",
                            ContentType = "image/png",
                            ImageName = "gen_sample.png"
                        }
                    },
                    new AiStepResult
                    {
                        StepName = "removebg",
                        Status = StepStatus.Success,
                        Message = "Background removed successfully",
                        Output = new ImageResult
                        {
                            ImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAUA...",
                            ContentType = "image/png",
                            ImageName = "nobg_gen_sample.png"
                        }
                    },
                    new AiStepResult
                    {
                        StepName = "realesrgan",
                        Status = StepStatus.Success,
                        Message = "Upscaled to 4x resolution",
                        Output = new ImageResult
                        {
                            ImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAUA...",
                            ContentType = "image/png",
                            ImageName = "upscale_nobg_gen_sample.png"
                        }
                    }
                },
                FinalResult = new ImageResult
                {
                    ImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAUA...",
                    ContentType = "image/png",
                    ImageName = "final_result.png"
                }
            };
        }
    }
}
