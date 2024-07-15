using EShop.Infrastructure.Command.Wallet;

namespace EShop.Wallet.DataProvider.Services;

public interface IWalletService
{
    Task<bool> AddFunds(AddFunds funds);
    Task<bool> DeductFunds(DeductFunds funds);
}