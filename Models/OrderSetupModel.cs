namespace DBCafeteria.Models;

public enum OrderType { ForMe, Gift }
public enum PickupType { Now, PreOrder }

public sealed class OrderSetupModel
{
    public int? BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public OrderType OrderType { get; set; } = OrderType.ForMe;
    public PickupType PickupType { get; set; } = PickupType.Now;
    public DateTime? PickupDateTime { get; set; }
}
