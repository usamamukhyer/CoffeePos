using System.Collections.ObjectModel;
using System.Windows.Input;
using DBCafeteria.Models;
using DBCafeteria.Services;

namespace DBCafeteria.ViewModels;

public sealed class OrderSetupViewModel : BaseViewModel
{
    private readonly OrderSessionService _session = OrderSessionService.Instance;
    private readonly BranchApiService _branchApiService = new();
    private readonly LocalBranchCacheService _branchCacheService = new();
    private BranchModel? _selectedBranch;
    private OrderType _orderType = OrderSessionService.Instance.OrderType;
    private PickupType _pickupType = OrderSessionService.Instance.PickupType;
    private DateTime _pickupDate = OrderSessionService.Instance.PickupDateTime?.Date ?? DateTime.Today;
    private TimeSpan _pickupTime = OrderSessionService.Instance.PickupDateTime?.TimeOfDay ?? DateTime.Now.AddMinutes(30).TimeOfDay;
    private string _errorMessage = string.Empty;
    private bool _isBusy;

    public ObservableCollection<BranchModel> Branches { get; } = [];

    public BranchModel? SelectedBranch
    {
        get => _selectedBranch;
        set
        {
            if (!SetProperty(ref _selectedBranch, value))
                return;

            if (value is null)
            {
                _session.BranchId = null;
                _session.BranchName = string.Empty;
                return;
            }

            _session.BranchId = value.Id;
            _session.BranchName = value.Name;
        }
    }

    public DateTime MinimumPickupDate => DateTime.Today;
    public bool IsPickupDateTimeVisible => PickupType == PickupType.PreOrder;
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
    public bool IsForMeSelected => OrderType == OrderType.ForMe;
    public bool IsGiftSelected => OrderType == OrderType.Gift;
    public bool IsNowSelected => PickupType == PickupType.Now;
    public bool IsPreOrderSelected => PickupType == PickupType.PreOrder;

    public OrderType OrderType
    {
        get => _orderType;
        set
        {
            if (!SetProperty(ref _orderType, value))
                return;
            _session.OrderType = value;
            OnPropertyChanged(nameof(IsForMeSelected));
            OnPropertyChanged(nameof(IsGiftSelected));
        }
    }

    public PickupType PickupType
    {
        get => _pickupType;
        set
        {
            if (!SetProperty(ref _pickupType, value))
                return;
            _session.PickupType = value;
            if (value == PickupType.Now)
                _session.PickupDateTime = null;
            else
                ApplyPickupDateTime();

            OnPropertyChanged(nameof(IsNowSelected));
            OnPropertyChanged(nameof(IsPreOrderSelected));
            OnPropertyChanged(nameof(IsPickupDateTimeVisible));
        }
    }

    public DateTime PickupDate
    {
        get => _pickupDate;
        set
        {
            if (SetProperty(ref _pickupDate, value))
                ApplyPickupDateTime();
        }
    }

    public TimeSpan PickupTime
    {
        get => _pickupTime;
        set
        {
            if (SetProperty(ref _pickupTime, value))
                ApplyPickupDateTime();
        }
    }

    public DateTime? SelectedPickupDateTime => PickupType == PickupType.PreOrder ? PickupDate.Date.Add(PickupTime) : null;

    public ICommand LoadBranchesCommand => new AsyncCommand(LoadBranchesAsync);
    public ICommand SelectOrderTypeCommand => new RelayCommand(value => OrderType = value?.ToString() == "Gift" ? OrderType.Gift : OrderType.ForMe);
    public ICommand SelectPickupCommand => new RelayCommand(value => PickupType = value?.ToString() == "PreOrder" ? PickupType.PreOrder : PickupType.Now);
    public ICommand ContinueCommand => new AsyncCommand(ContinueAsync);

    private async Task LoadBranchesAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            SetError(string.Empty);

            var cachedBranches = await _branchCacheService.GetBranchesAsync();
            ReplaceBranches(cachedBranches);
            _ = SyncBranchesAsync();
            if (cachedBranches.Count > 0)
                return;

            var branches = await _branchApiService.GetBranchesAsync();
            ReplaceBranches(branches);
        }
        catch (HttpRequestException)
        {
            SetError(NetworkErrorMessages.ApiUnavailable("Sucursales"));
        }
        catch (ApiException ex)
        {
            SetError(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SyncBranchesAsync()
    {
        try
        {
            var branches = await _branchApiService.GetBranchesAsync();
            await _branchCacheService.UpsertBranchesAsync(branches.Select(branch => new ApiBranch(branch.Id, branch.Name, branch.Address)));
        }
        catch (Exception ex) when (ex is HttpRequestException or ApiException)
        {
        }
    }

    private void ReplaceBranches(IEnumerable<BranchModel> branches)
    {
        Branches.Clear();
        foreach (var branch in branches)
            Branches.Add(branch);

        SelectedBranch = _session.BranchId is int branchId
            ? Branches.FirstOrDefault(branch => branch.Id == branchId) ?? Branches.FirstOrDefault()
            : Branches.FirstOrDefault();
    }

    private async Task ContinueAsync()
    {
        if (SelectedBranch is null)
        {
            SetError("Selecciona una sucursal.");
            return;
        }

        if (PickupType == PickupType.PreOrder)
        {
            ApplyPickupDateTime();
            if (_session.PickupDateTime is null)
            {
                SetError("Selecciona fecha y hora de recoleccion.");
                return;
            }

            if (_session.PickupDateTime <= DateTime.Now)
            {
                SetError("La fecha y hora de recoleccion debe ser futura.");
                return;
            }
        }

        if (PickupType == PickupType.Now)
            _session.PickupDateTime = null;

        SetError(string.Empty);
        await Shell.Current.GoToAsync(OrderType == OrderType.Gift ? nameof(Views.GiftDetailsPage) : nameof(Views.ShopByCategoryPage));
    }

    private void ApplyPickupDateTime()
    {
        _session.PickupDateTime = PickupType == PickupType.PreOrder ? PickupDate.Date.Add(PickupTime) : null;
        OnPropertyChanged(nameof(SelectedPickupDateTime));
    }

    private void SetError(string message)
    {
        ErrorMessage = message;
        OnPropertyChanged(nameof(HasError));
    }
}
