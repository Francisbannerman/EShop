namespace EShop.Infrastructure.Command.Wallet;

public class DeductFunds
{
    public string UserId { get; set; }
    public decimal DebitAmount { get; set; }
}