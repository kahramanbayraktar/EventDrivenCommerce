using AutoMapper;
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

        public CreateProductCommandHandler(IProductRepository repository, IMapper mapper, IEventPublisher eventPublisher)
        {
            _repository = repository;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        }

        public async Task<Result<ProductDTO>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = _mapper.Map<Product>(request.Product);
                product.CreatedAt = DateTime.UtcNow;

                var productAdded = await _repository.CreateAsync(product);
                var productDtoAdded = _mapper.Map<ProductDTO>(productAdded);

                // Publish ProductCreatedEvent
                var productCreatedEvent = new ProductCreatedEvent
                {
                    ProductId = productAdded.Id,
                    CreatedAt = productAdded.CreatedAt
                };
                await _eventPublisher.PublishAsync("product_created", productCreatedEvent);

                return Result<ProductDTO>.SuccessResult(productDtoAdded);
            }
            catch (Exception ex)
            {
                return Result<ProductDTO>.FailureResult("Failed adding product.", SharedKernel.Enums.ErrorCode.UnexpectedError, ex);
            }
        }
    }
}
