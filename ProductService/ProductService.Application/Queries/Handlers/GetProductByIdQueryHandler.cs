using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Application.Queries.Models;
using ProductService.Domain.Repositories;
using SharedKernel.Models;

namespace ProductService.Application.Queries.Handlers
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDTO>>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public GetProductByIdQueryHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<ProductDTO>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _repository.GetAsync(request.Id);
                if (product == null)
                {
                    //throw new ProductNotFoundException();,
                    return Result<ProductDTO>.FailureResult("Product not found.", SharedKernel.Enums.ErrorCode.NotFound);
                }

                var productDto = _mapper.Map<ProductDTO>(product);

                return Result<ProductDTO>.SuccessResult(productDto);
            }
            catch (Exception ex)
            {
                return Result<ProductDTO>.FailureResult($"Failed returning product {request.Id}.", SharedKernel.Enums.ErrorCode.UnexpectedError, ex);
            }
        }
    }
}
