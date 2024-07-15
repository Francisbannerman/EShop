using EShop.Infrastructure.Command.Wallet;
using EShop.Wallet.DataProvider.Repository;
using EShop.Wallet.DataProvider.Services;
using MassTransit;

namespace EShop.Wallet.Api.Handlers;

public class DeductFundsConsumer : IConsumer<DeductFunds>
{
    private IWalletService _walletService;
    public DeductFundsConsumer(IWalletService walletService)
    {
        _walletService = walletService;
    }
    
    public async Task Consume(ConsumeContext<DeductFunds> context)
    {
        var isDeducted = await _walletService.DeductFunds(context.Message);
        if (isDeducted)
        {
            await Task.CompletedTask;
        }
        else
        {
            throw new Exception("Funds Are Not Deducted. " +
                                "Try Again Some Other Time");
        }
    }
}