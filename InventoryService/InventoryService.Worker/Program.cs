using InventoryService.Application.Interfaces;
using InventoryService.Application.Mappings;
using InventoryService.Domain.Repositories;
using InventoryService.Infrastructure.Data.Context;
using InventoryService.Infrastructure.Data.Repositories;
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
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;

                    var connectionString = configuration.GetConnectionString("DefaultConnection");
                    services.AddDbContext<InventoryItemDbContext>(options =>
                        options.UseSqlServer(connectionString));

                    // Redis connection
                    var redisConnectionString = configuration.GetConnectionString("RedisConnection");
                    var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
                    services.AddSingleton<IConnectionMultiplexer>(redisConnection);

                    services.AddScoped<IInventoryItemService, InventoryItemService>();
                    services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();

                    // Add the Redis subscriber
                    services.AddSingleton<RedisEventSubscriber>();

                    services.AddAutoMapper(typeof(InventoryItemMappingProfile).Assembly);
                })
                .Build();

            Console.WriteLine("Hello, World!");

            using var scope = host.Services.CreateScope();
            var subscriber = scope.ServiceProvider.GetRequiredService<RedisEventSubscriber>();
            // Subscribe to product creation channel
            subscriber.Subscribe("product_created");

            Console.WriteLine("Inventory Service is subscribed to product creations");
            Console.ReadLine();
        }
    }
}
