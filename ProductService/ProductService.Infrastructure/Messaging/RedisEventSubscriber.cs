using StackExchange.Redis;

namespace ProductService.Infrastructure.Messaging
{
    public class RedisEventSubscriber
    {
        private readonly IConnectionMultiplexer _redisConnection;

        public RedisEventSubscriber(IConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
        }

        public void Subscribe(string topic, Action<string> handleMessage)
        {
            var subscriber = _redisConnection.GetSubscriber();
            subscriber.Subscribe(topic, (channel, message) =>
            {
                handleMessage(message);
            });
        }
    }
}
