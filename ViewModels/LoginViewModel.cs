using System.Windows.Input;
using DBCafeteria.Services;

namespace DBCafeteria.ViewModels;

public sealed class LoginViewModel : BaseViewModel
{
    private readonly CafeApiClient _apiClient = ApiClientFactory.Create();
    private string _emailOrPhone = string.Empty;
    private string _password = string.Empty;
    private string _validationMessage = string.Empty;

    public string EmailOrPhone { get => _emailOrPhone; set => SetProperty(ref _emailOrPhone, value); }
    public string Password { get => _password; set => SetProperty(ref _password, value); }
    public string ValidationMessage { get => _validationMessage; set => SetProperty(ref _validationMessage, value); }
    public bool HasValidationMessage => !string.IsNullOrWhiteSpace(ValidationMessage);

    public ICommand BackCommand { get; }
    public ICommand LoginCommand { get; }
    public ICommand SignupCommand { get; }
    public ICommand GuestCommand { get; }
    public ICommand ForgotPasswordCommand { get; }

    public LoginViewModel()
    {
        BackCommand = new AsyncCommand(async () => await Shell.Current.GoToAsync("//AuthLanding"));
        SignupCommand = new AsyncCommand(async () => await Shell.Current.GoToAsync(nameof(Views.SignupPage)));
        ForgotPasswordCommand = new AsyncCommand(async () => await Shell.Current.DisplayAlertAsync("Forgot Password", "Password reset will be connected later.", "OK"));
        GuestCommand = new AsyncCommand(async () =>
        {
            try
            {
                var response = await _apiClient.PostAsync<ApiGuestRequest, ApiAuthResponse>("auth/guest", new ApiGuestRequest(null, null));
                if (response is not null)
                    OrderSessionService.Instance.ApplyAuthResponse(response);
                else
                    OrderSessionService.Instance.ContinueAsGuest();
            }
            catch (HttpRequestException)
            {
                OrderSessionService.Instance.ContinueAsGuest();
            }

            await Shell.Current.GoToAsync("//OrderSetup");
        });
        LoginCommand = new AsyncCommand(async () =>
        {
            if (string.IsNullOrWhiteSpace(EmailOrPhone) || string.IsNullOrWhiteSpace(Password))
            {
                SetValidation("Please enter your email or phone and password.");
                return;
            }

            try
            {
                var response = await _apiClient.PostAsync<ApiLoginRequest, ApiAuthResponse>(
                    "auth/login",
                    new ApiLoginRequest(EmailOrPhone.Trim(), Password));

                if (response is null)
                {
                    SetValidation("Login failed. Please try again.");
                    return;
                }

                OrderSessionService.Instance.ApplyAuthResponse(response);
                SetValidation(string.Empty);
                await Shell.Current.GoToAsync("//OrderSetup");
            }
            catch (ApiException ex)
            {
                SetValidation(ex.Message);
            }
            catch (HttpRequestException)
            {
                SetValidation("API is not reachable. Please run the API on http://localhost:5126.");
            }
        });
    }

    private void SetValidation(string message)
    {
        ValidationMessage = message;
        OnPropertyChanged(nameof(HasValidationMessage));
    }
}
