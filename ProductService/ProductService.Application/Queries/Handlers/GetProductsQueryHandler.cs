using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Application.Queries.Models;
using ProductService.Domain.Repositories;
using SharedKernel.Enums;
using SharedKernel.Models;

namespace ProductService.Application.Queries.Handlers
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<IEnumerable<ProductDTO>>>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public GetProductsQueryHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<ProductDTO>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var products = await _repository.GetAsync();

                var productDtos = _mapper.Map<IEnumerable<ProductDTO>>(products);

                return Result<IEnumerable<ProductDTO>>.SuccessResult(productDtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<ProductDTO>>.FailureResult($"Failed returning products.", ErrorCode.UnexpectedError, ex);
            }
        }
    }
}
