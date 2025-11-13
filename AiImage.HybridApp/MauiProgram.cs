using AiImage.HybridApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace AiImage.HybridApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            MauiAppBuilder builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            #region appsettings.json configuration
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream? stream = assembly.GetManifestResourceStream(
                "AiImage.HybridApp.appsettings.json");
            if (stream is null)
                throw new FileNotFoundException(
                    "Missing embedded resource: appsettings.json");

            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonStream(stream!)  // safer for MAUI embedded files
                .Build();

            builder.Configuration.AddConfiguration(config);
            #endregion

            #region configure HttpClient
            builder.Services.AddSingleton(sp =>
            {
                SocketsHttpHandler handler = new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                    KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(30),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(15)
                };

                IConfiguration config = sp.GetRequiredService<IConfiguration>();
                string baseUrl = config["ApiBaseUrl"] ?? 
                throw new InvalidOperationException(
                    "ApiBaseUrl is missing from configuration.");

                return new HttpClient(handler)
                {
                    BaseAddress = new Uri(baseUrl),
                    Timeout = Timeout.InfiniteTimeSpan
                };
            });
            #endregion

            #region register modular services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IApiService, ApiService>();
            #endregion

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
