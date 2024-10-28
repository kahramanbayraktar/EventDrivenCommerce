using InventoryService.Application.Commands.Models;
using InventoryService.Domain.Repositories;
using MediatR;
using SharedKernel.Models;

namespace InventoryService.Application.Commands.Handlers
{
    public class UpdateInventoryItemHandler : IRequestHandler<UpdateInventoryItemCommand, Result<bool>>
    {
        private readonly IInventoryItemRepository _repository;

        public UpdateInventoryItemHandler(IInventoryItemRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(UpdateInventoryItemCommand request, CancellationToken cancellationToken)
        {
            var inventoryItem = await _repository.GetByProductIdAsync(request.ProductId);

            if (inventoryItem == null)
            {
                return Result<bool>.FailureResult($"Inventory item not found for product {request.ProductId}.");
            }

            inventoryItem.Quantity++;
            inventoryItem.LastUpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(inventoryItem);

            Console.WriteLine($"Updated inventory for ProductId: {request.ProductId}.");

            return Result<bool>.SuccessResult(true);
        }
    }
}
