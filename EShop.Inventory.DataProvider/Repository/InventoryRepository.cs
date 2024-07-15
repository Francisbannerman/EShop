using EShop.Infrastructure.Command.Inventory;

namespace EShop.Inventory.DataProvider.Repository;

public class InventoryRepository : IInventoryRepository
{
    public InventoryRepository()
    {
        
    }
    public Task<bool> AddStocks(AllocateProduct stock)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ReleaseStocks(ReleaseProduct stock)
    {
        throw new NotImplementedException();
    }
}