using EShop.Infrastructure.Inventory;

namespace EShop.Infrastructure.Command.Inventory;

public class AllocateProduct
{
    public List<StockItem> Items { get; set; }
}