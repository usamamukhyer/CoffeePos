namespace DBCafeteria.Models;

public sealed class GiftDetailsModel
{
    public int? RecipientCustomerId { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public string PersonalMessage { get; set; } = string.Empty;
}
