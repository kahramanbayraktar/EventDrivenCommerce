using MediatR;
using SharedKernel.Models;

namespace InventoryService.Application.Commands.Models
{
    public class CreateInventoryItemCommand : IRequest<Result<bool>>
    {
        public Guid ProductId { get; }
        public int InitialQuantity { get; set; } = 0;

        public CreateInventoryItemCommand(Guid productId)
        {
            ProductId = productId;
        }
    }
}
