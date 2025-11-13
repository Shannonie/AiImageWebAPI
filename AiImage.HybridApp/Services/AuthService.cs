namespace AiImage.HybridApp.Services
{
    public class AuthService : BaseApiService, IAuthService
    {
        public AuthService(HttpClient httpClient) : base(httpClient) { }

        public async Task<string?> LoginAsync(
            string username,
            string password)
        {
            Dictionary<string, string>? response =
                await PostAsync<Dictionary<string, string>>(
                    "/login", new { username, password });
            return response?.GetValueOrDefault("access_token");
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            using HttpRequestMessage request = 
                new HttpRequestMessage(HttpMethod.Get, "secure");
            AddAuthHeader(request, token);

            HttpResponseMessage response = await HttpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<string?> LoginAndValidateAsync(
            string username,
            string password)
        {
            string? token = await LoginAsync(username, password);
            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException(
                    "Login failed: no token returned.");

            bool isValid = await ValidateTokenAsync(token);
            if (!isValid)
                throw new UnauthorizedAccessException("Token validation failed.");

            return token;
        }
    }
}
