using SharedKernel.Models;

namespace InventoryService.Application.Interfaces
{
    public interface IInventoryItemService
    {
        Task<Result<bool>> UpdateInventory(string message);
    }
}
