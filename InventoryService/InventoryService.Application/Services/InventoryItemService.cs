using AutoMapper;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Repositories;
using ProductService.Domain.Events;
using SharedKernel.Models;
using System.Text.Json;

namespace InventoryService.Application.Interfaces
{
    public class InventoryItemService : IInventoryItemService
    {
        private readonly IInventoryItemRepository _repository;
        private readonly IMapper _mapper;

        public InventoryItemService(IInventoryItemRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<bool>> UpdateInventory(string message)
        {
            var productEvent = JsonSerializer.Deserialize<ProductCreatedEvent>(message);

            if (productEvent != null)
            {
                var inventoryItem = await _repository.GetByProductIdAsync(productEvent.ProductId);

                if (inventoryItem != null)
                {
                    inventoryItem.Quantity++;
                    inventoryItem.LastUpdatedAt = DateTime.UtcNow;

                    await _repository.UpdateAsync(inventoryItem);

                    Console.WriteLine($"Updated inventory for ProductId: {productEvent.ProductId}.");
                }
                else
                {
                    inventoryItem = new InventoryItem
                    {
                        ProductId = productEvent.ProductId,
                        Quantity = 0,
                        LastUpdatedAt = DateTime.UtcNow
                    };
                    await _repository.CreateAsync(inventoryItem);

                    Console.WriteLine($"Created new inventory item for ProductId: {productEvent.ProductId}.");
                }
            }

            return Result<bool>.SuccessResult(true);
        }
    }
}
