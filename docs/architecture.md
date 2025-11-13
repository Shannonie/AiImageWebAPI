## Request Flow
1. **User Input:** The MAUI/Blazor app collects user prompts and uploads images.
2. **API Gateway:** The API validates credentials (JWT planned) and routes tasks.
3. **Core Logic:** The AI pipeline runs (Stability / OpenAI / Remove.bg / ESRGAN).
4. **Response:** The processed image is returned as a Base64-encoded response.

## Dependencies
- `AiImageCore` — no external dependencies beyond `System.*` and `Microsoft.Extensions.*`
- `AiImageApi` — depends on ASP.NET Core and NuGet packages for AI API integration
- `AiImageHybridApp` — depends on MAUI, Blazor, and dependency injection from .NET 8
