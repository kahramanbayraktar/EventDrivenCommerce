using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.EntityFrameworkCore;
using ProductService.API.GraphQL.Mutations;
using ProductService.API.GraphQL.Queries;
using ProductService.API.GraphQL.Types;
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

            builder.Services
                .AddGraphQLServer()
                .AddQueryType<ProductQueries>()
                .AddMutationType<ProductMutations>()
                .AddType<ProductType>()
                .AddFiltering()
                .AddSorting();

            // ElasticSearch
            var handler = new HttpClientHandler
            {
                //ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            var elasticUri = builder.Configuration["ElasticSearch:Uri"];
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(elasticUri!)
            };
            var settings = new ElasticsearchClientSettings(httpClient.BaseAddress)
                .DefaultIndex("products")
                .Authentication(new BasicAuthentication("elastic", "Sor2-BAKtuQ1iWTMeMHa"));
            //.EnableDebugMode();
            var elasticClient = new ElasticsearchClient(settings);
            builder.Services.AddSingleton(elasticClient);

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

            app.MapGraphQL("/api/products/graphql");

            app.Run();
        }
    }
}
