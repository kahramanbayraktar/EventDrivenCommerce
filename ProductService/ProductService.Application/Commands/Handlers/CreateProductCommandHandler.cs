using AutoMapper;
using Elastic.Clients.Elasticsearch;
using MediatR;
using ProductService.Application.Commands.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using ProductService.Domain.Events;
using ProductService.Domain.Repositories;
using SharedKernel.Models;

namespace ProductService.Application.Commands.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDTO>>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;
        private readonly ElasticsearchClient _elasticClient;

        public CreateProductCommandHandler(IProductRepository repository, IMapper mapper, IEventPublisher eventPublisher, ElasticsearchClient elasticClient)
        {
            _repository = repository;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
            _elasticClient = elasticClient;
        }

        public async Task<Result<ProductDTO>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Execute this only once at the beginning of the project.
                //await IndexAllProducts();

                var product = _mapper.Map<Product>(request.Product);
                product.CreatedAt = DateTime.UtcNow;

                var createdProduct = await _repository.CreateAsync(product);

                // Index product in Elasticsearch
                var response = await _elasticClient.IndexAsync(createdProduct);
                if (!response.IsValidResponse)
                {
                    return Result<ProductDTO>.FailureResult("Filed to index product in Elasticsearch.");
                }

                // Publish ProductCreatedEvent
                var productCreatedEvent = new ProductCreatedEvent
                {
                    ProductId = createdProduct.Id,
                    CreatedAt = createdProduct.CreatedAt
                };
                await _eventPublisher.PublishAsync("product_created", productCreatedEvent);

                return Result<ProductDTO>.SuccessResult(_mapper.Map<ProductDTO>(createdProduct));
            }
            catch (Exception ex)
            {
                return Result<ProductDTO>.FailureResult("Failed to add product.", SharedKernel.Enums.ErrorCode.UnexpectedError, ex);
            }
        }

        private async Task IndexAllProducts()
        {
            try
            {
                var products = await _repository.GetAsync();
                // Index all existing products in Elasticsearch
                foreach (var product in products)
                {
                    var response = await _elasticClient.IndexAsync(product);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
