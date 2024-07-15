using EShop.Infrastructure.Command.Wallet;

namespace EShop.Wallet.DataProvider.Repository;

public interface IWalletRepository
{
    Task<bool> AddFunds(AddFunds funds);
    Task<bool> DeductFunds(DeductFunds funds);
}