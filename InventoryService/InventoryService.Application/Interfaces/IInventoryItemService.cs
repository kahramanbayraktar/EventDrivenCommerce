using InventoryService.Application.DTOs;
using SharedKernel.Models;

namespace InventoryService.Application.Interfaces
{
    public interface IInventoryItemService
    {
        Task<Result<InventoryItemDTO>> CreateInventoryItem(InventoryItemDTO itemDTO);
    }
}
