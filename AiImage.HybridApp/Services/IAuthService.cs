namespace AiImage.HybridApp.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(string username, string password);
        Task<bool> ValidateTokenAsync(string token);
        Task<string?> LoginAndValidateAsync(string username, string password);
    }
}
