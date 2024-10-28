using InventoryService.Application.Commands.Models;
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
                    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                    var configBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env}.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables(); // Allow environment variables to override configuration

                    var config = configBuilder.Build();

                    var connectionString = config.GetConnectionString("DefaultConnection");
                    services.AddDbContext<InventoryItemDbContext>(options =>
                        options.UseSqlServer(connectionString));

                    // Redis connection
                    var redisConnectionString = config.GetConnectionString("RedisConnection");
                    var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
                    services.AddSingleton<IConnectionMultiplexer>(redisConnection);

                    services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();

                    // Add the Redis subscriber
                    services.AddSingleton<RedisEventSubscriber>();

                    services.AddAutoMapper(typeof(InventoryItemMappingProfile).Assembly);
                    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateInventoryItemCommand).Assembly));
                })
                .Build();

            using var scope = host.Services.CreateScope();
            var subscriber = scope.ServiceProvider.GetRequiredService<RedisEventSubscriber>();
            // Subscribe to product creation channel
            subscriber.Subscribe("product_created");

            Console.WriteLine("Inventory Service is subscribed to product creations");
            Console.ReadLine();
        }
    }
}
