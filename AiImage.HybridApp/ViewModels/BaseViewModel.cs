using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AiImage.HybridApp.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isProcessing;
        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }

        private string? _errorMessage;
        public string? ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        protected bool SetProperty<T>(
            ref T field, 
            T value,
            [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(name);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected async Task RunSafeAsync(Func<Task> action)
        {
            try
            {
                ErrorMessage = null;
                IsProcessing = true;
                await action();
            }
            catch (Exception ex)
            {
                ErrorMessage = GetError(ex);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        protected virtual string GetError(Exception ex)
        {
            return ex switch
            {
                HttpRequestException => "Network error while contacting the server.",
                UnauthorizedAccessException => "Invalid credentials or no response.",
                TimeoutException => "The request timed out.",
                TaskCanceledException => "The operation was canceled.",
                _ => $"Unexpected error: {ex.Message}"
            };
        }
    }
}
