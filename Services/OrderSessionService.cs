using System.Collections.ObjectModel;
using DBCafeteria.Models;
using Microsoft.Maui.Storage;

namespace DBCafeteria.Services;

public sealed class OrderSessionService
{
    private const string SessionSavedKey = "session.saved";
    private const string SessionIsGuestKey = "session.isGuest";
    private const string SessionIsAuthenticatedKey = "session.isAuthenticated";
    private const string SessionCustomerIdKey = "session.customerId";
    private const string SessionCustomerNameKey = "session.customerName";
    private const string SessionCustomerEmailKey = "session.customerEmail";
    private const string SessionCustomerPhoneKey = "session.customerPhone";
    private const string SessionAccessTokenKey = "session.accessToken";
    private const string SessionRefreshTokenKey = "session.refreshToken";

    public static OrderSessionService Instance { get; } = new();

    private OrderSessionService()
    {
    }

    public bool IsGuest { get; set; } = true;
    public bool IsAuthenticated { get; set; }
    public int? CustomerId { get; set; }
    public string CustomerName { get; set; } = "Invitado";
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int? BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string SelectedLocation { get => BranchName; set => BranchName = value; }
    public OrderType OrderType { get; set; } = OrderType.ForMe;
    public PickupType PickupType { get; set; } = PickupType.Now;
    public DateTime? PickupDateTime { get; set; }
    public GiftDetailsModel GiftDetails { get; } = new();
    public int? GiftRecipientCustomerId { get => GiftDetails.RecipientCustomerId; set => GiftDetails.RecipientCustomerId = value; }
    public string GiftRecipientName { get => GiftDetails.RecipientName; set => GiftDetails.RecipientName = value; }
    public string GiftRecipientPhone { get => GiftDetails.RecipientPhone; set => GiftDetails.RecipientPhone = value; }
    public string GiftMessage { get => GiftDetails.PersonalMessage; set => GiftDetails.PersonalMessage = value; }
    public CategoryModel? SelectedCategory { get; set; }
    public ProductModel? SelectedProduct { get; set; }
    public ObservableCollection<BasketItemModel> BasketItems { get; } = new();
    public int BasketCount => BasketItems.Sum(item => item.Quantity);
    public decimal BasketTotal => BasketItems.Sum(item => item.Price * item.Quantity);
    public string CurrentClientOrderId { get; set; } = Guid.NewGuid().ToString("N");

    public void ContinueAsGuest(string name = "Invitado", string phone = "")
    {
        IsGuest = true;
        IsAuthenticated = false;
        CustomerId = null;
        CustomerName = string.IsNullOrWhiteSpace(name) ? "Invitado" : name.Trim();
        CustomerEmail = string.Empty;
        CustomerPhone = phone.Trim();
        AccessToken = string.Empty;
        RefreshToken = string.Empty;
        ClearPersistedSession();
    }

    public void Authenticate(string name, string email, string phone = "", int? customerId = null)
    {
        IsGuest = false;
        IsAuthenticated = true;
        CustomerId = customerId;
        CustomerName = string.IsNullOrWhiteSpace(name) ? "Cliente" : name.Trim();
        CustomerEmail = email.Trim();
        CustomerPhone = phone.Trim();
        _ = PersistAsync();
    }

    public void ApplyAuthResponse(ApiAuthResponse response)
    {
        IsGuest = response.IsGuest;
        IsAuthenticated = !response.IsGuest;
        CustomerId = response.CustomerId;
        CustomerName = response.CustomerName;
        CustomerEmail = response.Email ?? string.Empty;
        CustomerPhone = response.Phone ?? string.Empty;
        AccessToken = response.AccessToken;
        RefreshToken = response.RefreshToken;
        if (IsAuthenticated)
            _ = PersistAsync();
        else
            ClearPersistedSession();
    }

    public async Task<bool> RestoreAsync()
    {
        if (!Preferences.Get(SessionSavedKey, false))
            return false;

        if (Preferences.Get(SessionIsGuestKey, true) || !Preferences.Get(SessionIsAuthenticatedKey, false))
        {
            ClearPersistedSession();
            return false;
        }

        IsGuest = Preferences.Get(SessionIsGuestKey, true);
        IsAuthenticated = Preferences.Get(SessionIsAuthenticatedKey, false);

        var customerId = Preferences.Get(SessionCustomerIdKey, 0);
        CustomerId = customerId > 0 ? customerId : null;
        CustomerName = Preferences.Get(SessionCustomerNameKey, IsGuest ? "Invitado" : "Cliente");
        CustomerEmail = Preferences.Get(SessionCustomerEmailKey, string.Empty);
        CustomerPhone = Preferences.Get(SessionCustomerPhoneKey, string.Empty);
        try
        {
            AccessToken = await SecureStorage.GetAsync(SessionAccessTokenKey) ?? string.Empty;
            RefreshToken = await SecureStorage.GetAsync(SessionRefreshTokenKey) ?? string.Empty;
        }
        catch
        {
            AccessToken = string.Empty;
            RefreshToken = string.Empty;
        }

        return true;
    }

    public async Task PersistAsync()
    {
        Preferences.Set(SessionSavedKey, true);
        Preferences.Set(SessionIsGuestKey, IsGuest);
        Preferences.Set(SessionIsAuthenticatedKey, IsAuthenticated);
        Preferences.Set(SessionCustomerIdKey, CustomerId ?? 0);
        Preferences.Set(SessionCustomerNameKey, CustomerName);
        Preferences.Set(SessionCustomerEmailKey, CustomerEmail);
        Preferences.Set(SessionCustomerPhoneKey, CustomerPhone);

        try
        {
            if (string.IsNullOrWhiteSpace(AccessToken))
                SecureStorage.Remove(SessionAccessTokenKey);
            else
                await SecureStorage.SetAsync(SessionAccessTokenKey, AccessToken);

            if (string.IsNullOrWhiteSpace(RefreshToken))
                SecureStorage.Remove(SessionRefreshTokenKey);
            else
                await SecureStorage.SetAsync(SessionRefreshTokenKey, RefreshToken);
        }
        catch
        {
        }
    }

    public void ClearPersistedSession()
    {
        Preferences.Remove(SessionSavedKey);
        Preferences.Remove(SessionIsGuestKey);
        Preferences.Remove(SessionIsAuthenticatedKey);
        Preferences.Remove(SessionCustomerIdKey);
        Preferences.Remove(SessionCustomerNameKey);
        Preferences.Remove(SessionCustomerEmailKey);
        Preferences.Remove(SessionCustomerPhoneKey);
        SecureStorage.Remove(SessionAccessTokenKey);
        SecureStorage.Remove(SessionRefreshTokenKey);
    }
}
