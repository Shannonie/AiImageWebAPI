using AiImage.Core.Models;
using AiImage.HybridApp.Services;

namespace AiImage.HybridApp.ViewModels
{
    public class ProcessViewModel : AuthViewModel
    {
        private readonly IApiService _apiService;

        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(300);
        public AiTaskResponse? Response { get; private set; }
        public AiTaskRequest? LastRequest { get; private set; }

        public ProcessViewModel(IApiService apiService,
            IAuthService authService)
            : base(authService)
        {
            _apiService = apiService;
        }

        public async Task ProcessRequestAsync(AiTaskRequest request)
        {
            try
            {
                await RunSafeAsync(async () =>
                {
                    LastRequest = request;
                    Response = await ExecuteProcessAsync(request);
                    ValidateResponse(Response);
                });
                Response = await _apiService.ProcessImageAsync(request, Token!);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task<AiTaskResponse?> ExecuteProcessAsync(AiTaskRequest request)
        {
            using CancellationTokenSource cts = new CancellationTokenSource(_timeout);
            Task<AiTaskResponse?> task = _apiService.ProcessImageAsync(request, Token!);
            Task completed = await Task.WhenAny(task, Task.Delay(_timeout, cts.Token));
            if (completed != task)
                GetError(new TimeoutException());

            return await task;
        }

        private void ValidateResponse(AiTaskResponse? response)
        {
            string errMessage = "";

            if (response == null)
                errMessage = "No response received from server.";

            foreach (AiStepResult step in response!.Steps)
            {
                if (step.Status != StepStatus.Success)
                {
                    errMessage = $"[{step.StepName}] {step.Message}";
                    break;
                }
            }

            if (string.IsNullOrEmpty(errMessage) && response.FinalResult?.ImageBase64 == null)
                errMessage = "No image was returned from the server.";

            if(!string.IsNullOrEmpty(errMessage))
                throw new Exception(errMessage);
        }

        public async Task RetryLastActionAsync()
        {
            if (LastRequest is not null)
                await ProcessRequestAsync(LastRequest);
        }

        public void ClearResult()
        {
            Response = null;
            ErrorMessage = null;
            LastRequest = null;
        }
    }
}
