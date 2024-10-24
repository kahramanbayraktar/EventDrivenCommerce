using InventoryService.Domain.Entities;
using InventoryService.Domain.Repositories;
using InventoryService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Data.Repositories
{
    public class InventoryItemRepository : IInventoryItemRepository
    {
        private readonly InventoryItemDbContext _context;

        public InventoryItemRepository(InventoryItemDbContext context)
        {
            _context = context;
        }

        public async Task<InventoryItem?> GetByProductIdAsync(Guid id)
        {
            return await _context.InventoryItems.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreateAsync(InventoryItem item)
        {
            await _context.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(InventoryItem item)
        {
            _context.Update(item);
            await _context.SaveChangesAsync();
        }
    }
}
