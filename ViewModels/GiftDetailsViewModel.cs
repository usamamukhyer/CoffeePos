using System.Collections.ObjectModel;
using System.Windows.Input;
using DBCafeteria.Models;
using DBCafeteria.Services;

namespace DBCafeteria.ViewModels;

public sealed class GiftDetailsViewModel : BaseViewModel
{
    private readonly OrderSessionService _session = OrderSessionService.Instance;
    private readonly LocalCustomerCacheService _customerCacheService = new();
    private CancellationTokenSource? _searchDebounceCts;
    private CustomerSearchResultModel? _selectedRecipient;
    private string _recipientSearchText = OrderSessionService.Instance.GiftRecipientName;
    private string _recipientPhone = OrderSessionService.Instance.GiftRecipientPhone;
    private string _personalMessage = OrderSessionService.Instance.GiftMessage;
    private string _errorMessage = string.Empty;
    private bool _isRecipientPhoneReadOnly = true;
    private bool _isBusy;
    private bool _isSelectingRecipient;

    public ObservableCollection<CustomerSearchResultModel> CustomerSuggestions { get; } = [];

    public string RecipientSearchText
    {
        get => _recipientSearchText;
        set
        {
            if (!SetProperty(ref _recipientSearchText, value))
                return;

            OnPropertyChanged(nameof(PreviewRecipientName));

            if (_isSelectingRecipient)
                return;

            ClearSelectedRecipient();
            _ = DebounceSearchAsync(value);
        }
    }

    public CustomerSearchResultModel? SelectedRecipient
    {
        get => _selectedRecipient;
        set
        {
            if (SetProperty(ref _selectedRecipient, value))
                OnPropertyChanged(nameof(HasSelectedRecipient));
        }
    }

    public string RecipientPhone
    {
        get => _recipientPhone;
        set
        {
            if (SetProperty(ref _recipientPhone, value))
                _session.GiftRecipientPhone = value;
        }
    }

    public string PersonalMessage
    {
        get => _personalMessage;
        set
        {
            if (!SetProperty(ref _personalMessage, value))
                return;

            _session.GiftMessage = value;
            OnPropertyChanged(nameof(PreviewMessage));
        }
    }

    public bool IsRecipientPhoneReadOnly
    {
        get => _isRecipientPhoneReadOnly;
        set => SetProperty(ref _isRecipientPhoneReadOnly, value);
    }

    public bool HasSelectedRecipient => SelectedRecipient is not null;
    public bool HasSuggestions => CustomerSuggestions.Count > 0;
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public string PreviewRecipientName => string.IsNullOrWhiteSpace(RecipientSearchText) ? "Nombre del destinatario" : RecipientSearchText;
    public string PreviewMessage => string.IsNullOrWhiteSpace(PersonalMessage) ? "Tu mensaje aparecera aqui..." : PersonalMessage;

    public ICommand SearchRecipientCommand => new AsyncCommand(() => SearchRecipientAsync(RecipientSearchText));
    public ICommand SelectRecipientCommand => new RelayCommand(SelectRecipient);
    public ICommand ConfirmGiftCommand => new AsyncCommand(ConfirmGiftAsync);
    public ICommand ContinueCommand => ConfirmGiftCommand;
    public ICommand BackCommand { get; } = new AsyncCommand(async () => await Shell.Current.GoToAsync(".."));

    private async Task DebounceSearchAsync(string query)
    {
        _searchDebounceCts?.Cancel();
        _searchDebounceCts?.Dispose();
        _searchDebounceCts = new CancellationTokenSource();
        var token = _searchDebounceCts.Token;

        try
        {
            await Task.Delay(300, token);
            await SearchRecipientAsync(query, token);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task SearchRecipientAsync(string query, CancellationToken cancellationToken = default)
    {
        CustomerSuggestions.Clear();
        OnPropertyChanged(nameof(HasSuggestions));

        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
            return;

        try
        {
            IsBusy = true;
            SetError(string.Empty);
            var customers = await _customerCacheService.SearchCustomersAsync(query, _session.CustomerId, cancellationToken);
            foreach (var customer in customers)
                CustomerSuggestions.Add(customer);

            OnPropertyChanged(nameof(HasSuggestions));
        }
        catch (Exception)
        {
            SetError("No se pudo buscar clientes guardados.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void SelectRecipient(object? value)
    {
        if (value is not CustomerSearchResultModel customer)
            return;

        if (_session.CustomerId == customer.Id)
        {
            SetError("Selecciona a otra persona para enviar el regalo.");
            return;
        }

        _isSelectingRecipient = true;
        SelectedRecipient = customer;
        RecipientSearchText = customer.Name;
        RecipientPhone = customer.Phone;
        IsRecipientPhoneReadOnly = true;
        CustomerSuggestions.Clear();
        OnPropertyChanged(nameof(HasSuggestions));
        SetError(string.Empty);

        _session.GiftRecipientCustomerId = customer.Id;
        _session.GiftRecipientName = customer.Name;
        _session.GiftRecipientPhone = customer.Phone;
        _isSelectingRecipient = false;
    }

    private async Task ConfirmGiftAsync()
    {
        if (SelectedRecipient is null && _session.GiftRecipientCustomerId is null)
        {
            SetError("Selecciona un cliente existente.");
            return;
        }

        if (_session.GiftRecipientCustomerId == _session.CustomerId)
        {
            SetError("Selecciona a otra persona para enviar el regalo.");
            return;
        }

        if (SelectedRecipient is not null)
        {
            _session.GiftRecipientCustomerId = SelectedRecipient.Id;
            _session.GiftRecipientName = SelectedRecipient.Name;
            _session.GiftRecipientPhone = SelectedRecipient.Phone;
        }

        _session.GiftMessage = PersonalMessage;
        SetError(string.Empty);
        await Shell.Current.GoToAsync(nameof(Views.ShopByCategoryPage));
    }

    private void ClearSelectedRecipient()
    {
        SelectedRecipient = null;
        _session.GiftRecipientCustomerId = null;
        _session.GiftRecipientName = RecipientSearchText;
        _session.GiftRecipientPhone = string.Empty;
        RecipientPhone = string.Empty;
        IsRecipientPhoneReadOnly = true;
    }

    private void SetError(string message)
    {
        ErrorMessage = message;
        OnPropertyChanged(nameof(HasError));
    }
}
