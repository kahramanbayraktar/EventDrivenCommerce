using InventoryService.Domain.Entities;

namespace InventoryService.Domain.Repositories
{
    public interface IInventoryItemRepository
    {
        Task<InventoryItem?> GetByProductIdAsync(Guid id);
        Task CreateAsync(InventoryItem item);
        Task UpdateAsync(InventoryItem item);
    }
}
