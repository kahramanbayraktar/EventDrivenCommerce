using InventoryService.Infrastructure.Data.Context;
using InventoryService.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace InventoryService.Worker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Redis connection
                    var redisConnectionString = context.Configuration.GetConnectionString("RedisConnection");
                    var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
                    services.AddSingleton<IConnectionMultiplexer>(redisConnection);

                    // Add the Redis subscriber
                    services.AddSingleton<RedisEventSubscriber>();

                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                    services.AddDbContext<InventoryItemDbContext>(options =>
                        options.UseSqlServer(connectionString));
                });

            var app = builder.Build();

            Console.WriteLine("Hello, World!");

            using var scope = app.Services.CreateScope();
            var subscriber = scope.ServiceProvider.GetRequiredService<RedisEventSubscriber>();
            // Subscribe to product creation channel
            subscriber.Subscribe("product_created");

            Console.WriteLine("Inventory Service is subscribed to product creations");
            Console.ReadLine();
        }
    }
}
