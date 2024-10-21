using InventoryService.Domain.Entities;
using InventoryService.Domain.Repositories;
using InventoryService.Infrastructure.Data.Context;

namespace InventoryService.Infrastructure.Data.Repositories
{
    public class InventoryItemRepository : IInventoryItemRepository
    {
        private readonly InventoryItemDbContext _context;

        public InventoryItemRepository(InventoryItemDbContext context)
        {
            _context = context;
        }

        public async Task<InventoryItem> CreateAsync(InventoryItem item)
        {
            await _context.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
