namespace EShop.Infrastructure.Command.Wallet;

public class AddFunds
{
    public string UserId { get; set; }
    public decimal CreditAmount { get; set; }
}