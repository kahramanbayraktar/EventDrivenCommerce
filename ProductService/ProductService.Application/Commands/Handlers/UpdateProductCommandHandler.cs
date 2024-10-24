using AutoMapper;
using MediatR;
using ProductService.Application.Commands.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Domain.Events;
using ProductService.Domain.Repositories;
using SharedKernel.Models;

namespace ProductService.Application.Commands.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductDTO>>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public UpdateProductCommandHandler(IProductRepository repository, IMapper mapper, IEventPublisher eventPublisher)
        {
            _repository = repository;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        }

        public async Task<Result<ProductDTO>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingProduct = await _repository.GetAsync(request.Product.Id);
                if (existingProduct == null)
                {
                    return Result<ProductDTO>.FailureResult("Product not found.", SharedKernel.Enums.ErrorCode.NotFound);
                }

                _mapper.Map(request.Product, existingProduct);
                existingProduct.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(existingProduct);
                var productDtoUpdated = _mapper.Map<ProductDTO>(existingProduct);

                // Publish ProductUpdatedEvent
                var productUpdatedEvent = new ProductUpdatedEvent
                {
                    ProductId = existingProduct.Id,
                    UpdatedAt = existingProduct.UpdatedAt
                };
                await _eventPublisher.PublishAsync("product_updated", productUpdatedEvent);

                return Result<ProductDTO>.SuccessResult(productDtoUpdated);
            }
            catch (Exception ex)
            {
                return Result<ProductDTO>.FailureResult("Failed updating product.", SharedKernel.Enums.ErrorCode.UnexpectedError, ex);
            }
        }
    }
}
