using System.Windows.Input;
using DBCafeteria.Services;

namespace DBCafeteria.ViewModels;

public sealed class GuestDetailsViewModel : BaseViewModel
{
    private static readonly CafeApiClient ApiClient = ApiClientFactory.Create();
    private string _guestName = string.Empty;
    private string _guestPhone = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;

    public string GuestName { get => _guestName; set => SetProperty(ref _guestName, value); }
    public string GuestPhone { get => _guestPhone; set => SetProperty(ref _guestPhone, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

    public ICommand BackCommand { get; } = new AsyncCommand(async () => await Shell.Current.GoToAsync(".."));
    public ICommand ContinueCommand => new AsyncCommand(ContinueAsync);

    private async Task ContinueAsync()
    {
        if (string.IsNullOrWhiteSpace(GuestName))
        {
            SetError("Ingresa tu nombre para continuar como invitado.");
            return;
        }

        try
        {
            IsBusy = true;
            SetError(string.Empty);

            var response = await ApiClient.PostAsync<ApiGuestRequest, ApiAuthResponse>("auth/guest", new ApiGuestRequest(GuestName.Trim(), GuestPhone.Trim()));
            if (response is not null)
                OrderSessionService.Instance.ApplyAuthResponse(response);
            else
                OrderSessionService.Instance.ContinueAsGuest(GuestName.Trim(), GuestPhone.Trim());
        }
        catch (HttpRequestException)
        {
            OrderSessionService.Instance.ContinueAsGuest(GuestName.Trim(), GuestPhone.Trim());
        }
        finally
        {
            IsBusy = false;
        }

        await Shell.Current.GoToAsync("//OrderSetup");
    }

    private void SetError(string message)
    {
        ErrorMessage = message;
        OnPropertyChanged(nameof(HasError));
    }
}
