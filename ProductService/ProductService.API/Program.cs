using Microsoft.EntityFrameworkCore;
using ProductService.Application.Commands.Models;
using ProductService.Application.Interfaces;
using ProductService.Application.Mappings;
using ProductService.Domain.Repositories;
using ProductService.Infrastructure.Data.Context;
using ProductService.Infrastructure.Data.Repositories;
using ProductService.Infrastructure.Messaging;
using ProductService.Infrastructure.Messaging.Publishers;
using StackExchange.Redis;

namespace ProductService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ProductDbContext>(options => options.UseSqlServer(connectionString));

            var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");
            var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString!);
            builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);
            builder.Services.AddSingleton<RedisEventSubscriber>();

            builder.Services.AddScoped<IProductRepository, ProductRepository>();

            builder.Services.AddScoped<IEventPublisher, RedisEventPublisher>();

            builder.Services.AddAutoMapper(typeof(ProductMappingProfile).Assembly);
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));

            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
            ProductDbContext.SeedDatabase(context);

            // Subscribe to events after app startup
            var subscriber = app.Services.GetRequiredService<RedisEventSubscriber>();
            subscriber.Subscribe("product_created", message =>
            {
                Console.WriteLine($"Received event: {message}");
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
