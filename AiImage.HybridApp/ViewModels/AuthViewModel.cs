using AiImage.HybridApp.Services;

namespace AiImage.HybridApp.ViewModels
{
    public class AuthViewModel : BaseViewModel
    {
        protected readonly IAuthService _auth;
        private string _username = "demo";
        private string _password = "password";

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string? Token { get; private set; }
        public string? LoginMessage { get; private set; }

        public AuthViewModel(IAuthService authService)
        {
            _auth = authService;
        }

        public async Task<bool> LoginAndValidateAsync()
        {
            LoginMessage = null;
            ErrorMessage = null;

            return await RunLoginAndValidateSafeAsync(async () =>
            {
                Token = await _auth.LoginAndValidateAsync(Username, Password);
                if (string.IsNullOrEmpty(Token))
                    GetError(new UnauthorizedAccessException());
            });
        }

        private async Task<bool> RunLoginAndValidateSafeAsync(Func<Task> action)
        {
            try
            {
                await action();
                return true;
            }
            catch (Exception ex)
            {
                LoginMessage = GetError(ex);
                return false;
            }
        }
    }
}
