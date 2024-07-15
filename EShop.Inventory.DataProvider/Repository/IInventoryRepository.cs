using EShop.Infrastructure.Command.Inventory;

namespace EShop.Inventory.DataProvider.Repository;

public interface IInventoryRepository
{
    Task<bool> AddStocks(AllocateProduct stock);
    Task<bool> ReleaseStocks(ReleaseProduct stock);
}