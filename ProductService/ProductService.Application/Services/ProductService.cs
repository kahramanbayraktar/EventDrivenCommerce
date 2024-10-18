using AutoMapper;
using ProductService.Application.DTOs;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces;
using ProductService.Domain.Repositories;
using SharedKernel.Models;

namespace ProductService.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<ProductDTO>> GetProductById(Guid id)
        {
            try
            {
                var product = await _repository.GetAsync(id);

                if (product == null)
                {
                    throw new ProductNotFoundException();
                }

                var productDto = _mapper.Map<ProductDTO>(product);

                return Result<ProductDTO>.SuccessResult(productDto);
            }
            catch (Exception ex)
            {
                return Result<ProductDTO>.FailureResult($"Failed returning product {id}.", ex);
            }
        }
    }
}
