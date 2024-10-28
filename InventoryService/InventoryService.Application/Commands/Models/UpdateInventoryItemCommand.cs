using MediatR;
using SharedKernel.Models;

namespace InventoryService.Application.Commands.Models
{
    public class UpdateInventoryItemCommand : IRequest<Result<bool>>
    {
        public Guid ProductId { get; }
        public DateTime LastUpdatedAt { get; set; }

        public UpdateInventoryItemCommand(Guid productId)
        {
            ProductId = productId;
            LastUpdatedAt = DateTime.UtcNow;
        }
    }
}
