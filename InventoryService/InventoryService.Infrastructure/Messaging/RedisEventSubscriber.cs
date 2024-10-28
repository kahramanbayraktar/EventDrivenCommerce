using InventoryService.Application.Commands.Models;
using InventoryService.Domain.Repositories;
using MediatR;
using ProductService.Domain.Events;
using StackExchange.Redis;
using System.Text.Json;

namespace InventoryService.Infrastructure.Messaging
{
    public class RedisEventSubscriber
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IInventoryItemRepository _inventoryItemRepository;
        private readonly IMediator _mediator;

        public RedisEventSubscriber(IConnectionMultiplexer connectionMultiplexer, IInventoryItemRepository inventoryItemRepository, IMediator mediator)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _inventoryItemRepository = inventoryItemRepository;
            _mediator = mediator;
        }

        public void Subscribe(string channel)
        {
            var subscriber = _connectionMultiplexer.GetSubscriber();
            subscriber.Subscribe(channel, (redisChannel, message) =>
            {
                Console.WriteLine($"Received event: {message}");

                HandleProductEvent(message);
            });
        }

        public async void HandleProductEvent(string message)
        {
            Console.WriteLine($"Handling event: {message}");

            var productEvent = JsonSerializer.Deserialize<ProductCreatedEvent>(message);

            if (productEvent != null)
            {
                var existingItem = await _inventoryItemRepository.GetByProductIdAsync(productEvent.ProductId);

                if (existingItem == null)
                {
                    await _mediator.Send(new CreateInventoryItemCommand(productEvent.ProductId));
                }
                else
                {
                    await _mediator.Send(new UpdateInventoryItemCommand(productEvent.ProductId));
                }
            }
        }
    }
}
