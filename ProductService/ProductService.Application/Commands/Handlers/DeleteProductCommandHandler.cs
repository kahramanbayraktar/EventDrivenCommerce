using AutoMapper;
using MediatR;
using ProductService.Application.Commands.Models;
using ProductService.Application.Interfaces;
using ProductService.Domain.Events;
using ProductService.Domain.Repositories;
using SharedKernel.Models;

namespace ProductService.Application.Commands.Handlers
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<bool>>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public DeleteProductCommandHandler(IProductRepository repository, IMapper mapper, IEventPublisher eventPublisher)
        {
            _repository = repository;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        }

        public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _repository.GetAsync(request.Id);
                if (product == null)
                {
                    return Result<bool>.FailureResult("Product not found.", SharedKernel.Enums.ErrorCode.NotFound);
                }

                await _repository.DeleteAsync(product);

                // Publish ProductDeletedEvent
                var productDeletedEvent = new ProductDeletedEvent
                {
                    ProductId = product.Id,
                    DeletedAt = DateTime.UtcNow // TODO: product.UpdatedAt
                };
                await _eventPublisher.PublishAsync("product_deleted", productDeletedEvent);

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult("Failed deleting product.", SharedKernel.Enums.ErrorCode.UnexpectedError, ex);
            }
        }
    }
}
