using AiImageApi.Models;
using AiImageApi.Services;
using AiImageApi.SwaggerExamples;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Threading.Tasks;

namespace AiImageApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _service;

        public ImageController(IImageService service)
        {
            _service = service;
        }

        [HttpPost("process")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(AiTaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Run AI image pipeline",
            Description = "Executes a sequence of steps on the input image/prompt.")]
        public async Task<ActionResult<AiTaskResponse>> RunProcess(
            [FromBody] AiTaskRequest request,
            CancellationToken ct)
        {
            if (request == null || request.Payload == null)
                return BadRequest(new { error = "Invalid request body" });

            AiTaskResponse result = await _service.ExecuteAsync(request, ct);

            return result;
        }
    }
}
