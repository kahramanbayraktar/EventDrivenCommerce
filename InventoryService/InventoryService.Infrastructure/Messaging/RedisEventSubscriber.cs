using InventoryService.Application.Interfaces;
using StackExchange.Redis;

namespace InventoryService.Infrastructure.Messaging
{
    public class RedisEventSubscriber
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IInventoryItemService _inventoryItemService;

        public RedisEventSubscriber(IConnectionMultiplexer connectionMultiplexer, IInventoryItemService inventoryItemService)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _inventoryItemService = inventoryItemService;
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

        public void HandleProductEvent(string message)
        {
            Console.WriteLine($"Handling event: {message}");
            // Implement inventory service logic here

            _inventoryItemService.UpdateInventory(message);
        }
    }
}
