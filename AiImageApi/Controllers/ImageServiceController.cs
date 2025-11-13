using AiImage.Core.Models;
using AiImage.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AiImage.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _service;
        private readonly IConfiguration _config;
        
        public ImageController(IImageService service, IConfiguration config)
        {
            _service = service;
            _config = config;
        }

        [Authorize]
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
            bool enable = _config.GetValue<bool>("Features:EnableImageProcessing");
            if (!enable)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { message = "Login before using AI image processing." });
            }

            if (request == null || request.Payload == null)
                return BadRequest(new { error = "Invalid request body" });

            AiTaskResponse result = await _service.ExecuteAsync(request, ct);

            return result;
        }
    }
}
