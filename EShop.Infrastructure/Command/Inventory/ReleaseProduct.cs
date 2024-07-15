using EShop.Infrastructure.Inventory;

namespace EShop.Infrastructure.Command.Inventory;

public class ReleaseProduct
{
    public List<StockItem> Items { get; set; }
}