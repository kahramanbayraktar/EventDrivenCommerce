using InventoryService.Domain.Entities;

namespace InventoryService.Domain.Repositories
{
    public interface IInventoryItemRepository
    {
        Task<InventoryItem> CreateAsync(InventoryItem item);
    }
}
