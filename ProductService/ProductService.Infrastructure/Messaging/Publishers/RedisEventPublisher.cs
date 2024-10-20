using ProductService.Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace ProductService.Infrastructure.Messaging.Publishers
{
    public class RedisEventPublisher : IEventPublisher
    {
        private readonly IConnectionMultiplexer _redisConnection;

        public RedisEventPublisher(IConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
        }

        public async Task PublishAsync<TEvent>(string topic, TEvent eventMessage)
        {
            var jsonMessage = JsonSerializer.Serialize(eventMessage);
            var publisher = _redisConnection.GetSubscriber();
            await publisher.PublishAsync(topic, jsonMessage); // TODO: string or RedisChannel?
        }
    }
}
