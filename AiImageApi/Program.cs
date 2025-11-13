using AiImage.Api.SwaggerExamples;
using AiImage.Core.Models;
using AiImage.Core.Services;
using AiImage.Core.Services.Clients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

#region HttpClient + DI
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
builder.Services.AddSingleton<JwtService>();
#endregion

#region JWT auth
IConfigurationSection jwtSection = builder.Configuration.GetSection("Jwt");
byte[] keyBytesArray = Encoding.UTF8.GetBytes(jwtSection["Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(keyBytesArray)
        };
    });

builder.Services.AddAuthorization();
#endregion

#region Swagger settings
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

    // dd JWT Bearer authentication info for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer <your_token_here>"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    c.ExampleFilters();
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<AiTaskRequestExample>();
#endregion

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        c.RoutePrefix = string.Empty; // Swagger at root URL
    });
}

app.UseHttpsRedirection();
app.UseAuthentication(); // JWT auth
app.UseAuthorization();

#region Login endpoint
Dictionary<string, string> users = new Dictionary<string, string> { { "demo", "password" } };
app.MapPost("/login", (UserLogin user, JwtService jwtService) =>
{
    if (users.TryGetValue(user.Username, out var pwd) && pwd == user.Password)
    {
        string token = jwtService.GenerateToken(user.Username);
        return Results.Ok(new { access_token = token });
    }
    return Results.Unauthorized();
});

// Protected endpoint
app.MapGet("/secure", [Authorize] () =>
{
    return Results.Ok(new { message = "You are authenticated!" });
});
#endregion

app.MapControllers();
app.Run();
