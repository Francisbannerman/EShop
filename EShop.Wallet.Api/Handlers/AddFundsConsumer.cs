using EShop.Infrastructure.Command.Wallet;
using EShop.Wallet.DataProvider.Repository;
using EShop.Wallet.DataProvider.Services;
using MassTransit;
using Exception = System.Exception;

namespace EShop.Wallet.Api.Handlers;

public class AddFundsConsumer : IConsumer<AddFunds>
{
    private IWalletService _walletService;
    public AddFundsConsumer(IWalletService walletService)
    {
        _walletService = walletService;
    }
    public async Task Consume(ConsumeContext<AddFunds> context)
    {
        var isAdded = await _walletService.AddFunds(context.Message);
        if (isAdded)
        {
            await Task.CompletedTask;
        }
        else
        {
            throw new Exception("New Funds Are Not Added. " +
                                "Try Again Some Other Time");
        }
    }
}