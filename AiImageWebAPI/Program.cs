using AiImageApi.Services;
using AiImageApi.Services.Clients;
using AiImageApi.SwaggerExamples;
using Swashbuckle.AspNetCore.Filters;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AI Image API",
        Version = "v1",
        Description = "API for image processing"
    });
    c.ExampleFilters();
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<AiTaskRequestExample>();
// Swagger / OpenAPI

// HttpClient + DI
builder.Services.AddHttpClient<StabilityAIClient>();
builder.Services.AddHttpClient<OpenAIClient>();
builder.Services.AddHttpClient<RemoveBgClient>();
builder.Services.AddHttpClient<RealESRGANClient>();

builder.Services.AddTransient<IImageClient, StabilityAIClient>();
builder.Services.AddTransient<IImageClient, OpenAIClient>();
builder.Services.AddTransient<IImageClient, RemoveBgClient>();
builder.Services.AddTransient<IImageClient, RealESRGANClient>();
builder.Services.AddTransient<IImageClient, ImageSharpClient>();

builder.Services.AddSingleton<IImageService, ImageService>();
// HttpClient + DI

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        c.RoutePrefix = string.Empty; // Swagger at root URL
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
