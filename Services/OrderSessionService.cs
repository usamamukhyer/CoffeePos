using System.Collections.ObjectModel;
using DBCafeteria.Models;

namespace DBCafeteria.Services;

public sealed class OrderSessionService
{
    public static OrderSessionService Instance { get; } = new();

    private OrderSessionService()
    {
    }

    public bool IsGuest { get; set; } = true;
    public bool IsAuthenticated { get; set; }
    public int? CustomerId { get; set; }
    public string CustomerName { get; set; } = "Guest";
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

    public void ContinueAsGuest(string name = "Guest", string phone = "")
    {
        IsGuest = true;
        IsAuthenticated = false;
        CustomerId = null;
        CustomerName = string.IsNullOrWhiteSpace(name) ? "Guest" : name.Trim();
        CustomerEmail = string.Empty;
        CustomerPhone = phone.Trim();
        AccessToken = string.Empty;
        RefreshToken = string.Empty;
    }

    public void Authenticate(string name, string email, string phone = "", int? customerId = null)
    {
        IsGuest = false;
        IsAuthenticated = true;
        CustomerId = customerId;
        CustomerName = string.IsNullOrWhiteSpace(name) ? "Coffee Lover" : name.Trim();
        CustomerEmail = email.Trim();
        CustomerPhone = phone.Trim();
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
    }
}
