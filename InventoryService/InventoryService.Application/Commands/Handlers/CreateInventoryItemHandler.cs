using InventoryService.Application.Commands.Models;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Repositories;
using MediatR;
using SharedKernel.Models;

namespace InventoryService.Application.Commands.Handlers
{
    public class CreateInventoryItemHandler : IRequestHandler<CreateInventoryItemCommand, Result<bool>>
    {
        private readonly IInventoryItemRepository _repository;

        public CreateInventoryItemHandler(IInventoryItemRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(CreateInventoryItemCommand request, CancellationToken cancellationToken)
        {
            var inventoryItem = new InventoryItem
            {
                ProductId = request.ProductId,
                Quantity = 0,
                LastUpdatedAt = DateTime.UtcNow
            };
            await _repository.CreateAsync(inventoryItem);

            Console.WriteLine($"Created new inventory item for ProductId: {request.ProductId}.");

            return Result<bool>.SuccessResult(true);
        }
    }
}
