using System.Windows.Input;
using DBCafeteria.Services;

namespace DBCafeteria.ViewModels;

public sealed class SignupViewModel : BaseViewModel
{
    private readonly CafeApiClient _apiClient = ApiClientFactory.Create();
    private string _fullName = string.Empty;
    private string _email = string.Empty;
    private string _phoneNumber = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _validationMessage = string.Empty;

    public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }
    public string Email { get => _email; set => SetProperty(ref _email, value); }
    public string PhoneNumber { get => _phoneNumber; set => SetProperty(ref _phoneNumber, value); }
    public string Password { get => _password; set => SetProperty(ref _password, value); }
    public string ConfirmPassword { get => _confirmPassword; set => SetProperty(ref _confirmPassword, value); }
    public string ValidationMessage { get => _validationMessage; set => SetProperty(ref _validationMessage, value); }
    public bool HasValidationMessage => !string.IsNullOrWhiteSpace(ValidationMessage);

    public ICommand BackCommand { get; }
    public ICommand CreateAccountCommand { get; }
    public ICommand LoginCommand { get; }
    public ICommand GuestCommand { get; }

    public SignupViewModel()
    {
        BackCommand = new AsyncCommand(async () => await Shell.Current.GoToAsync("//AuthLanding"));
        LoginCommand = new AsyncCommand(async () => await Shell.Current.GoToAsync(nameof(Views.LoginPage)));
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
        CreateAccountCommand = new AsyncCommand(async () =>
        {
            var error = Validate();
            if (!string.IsNullOrWhiteSpace(error))
            {
                SetValidation(error);
                return;
            }

            try
            {
                var response = await _apiClient.PostAsync<ApiSignupRequest, ApiAuthResponse>(
                    "auth/signup",
                    new ApiSignupRequest(FullName.Trim(), EmptyToNull(Email), EmptyToNull(PhoneNumber), Password));

                if (response is null)
                {
                    SetValidation("Account creation failed. Please try again.");
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

    private string Validate()
    {
        if (string.IsNullOrWhiteSpace(FullName))
            return "Full name is required.";

        if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(PhoneNumber))
            return "Email or phone number is required.";

        if (string.IsNullOrWhiteSpace(Password))
            return "Password is required.";

        if (Password != ConfirmPassword)
            return "Confirm password must match password.";

        return string.Empty;
    }

    private void SetValidation(string message)
    {
        ValidationMessage = message;
        OnPropertyChanged(nameof(HasValidationMessage));
    }

    private static string? EmptyToNull(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
