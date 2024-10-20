using AutoMapper;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
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
                    //throw new ProductNotFoundException();,
                    return Result<ProductDTO>.FailureResult("Product not found.", SharedKernel.Enums.ErrorCode.NotFound);
                }

                var productDto = _mapper.Map<ProductDTO>(product);

                return Result<ProductDTO>.SuccessResult(productDto);
            }
            catch (Exception ex)
            {
                return Result<ProductDTO>.FailureResult($"Failed returning product {id}.", SharedKernel.Enums.ErrorCode.UnexpectedError, ex);
            }
        }

        public async Task<Result<ProductDTO>> CreateProduct(ProductDTO productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);
                product.CreatedAt = DateTime.UtcNow;

                var productAdded = await _repository.CreateAsync(product);
                var productDtoAdded = _mapper.Map<ProductDTO>(productAdded);
                return Result<ProductDTO>.SuccessResult(productDtoAdded);
            }
            catch (Exception ex)
            {
                return Result<ProductDTO>.FailureResult("Failed adding product.", SharedKernel.Enums.ErrorCode.UnexpectedError, ex);
            }
        }

        public async Task<Result<ProductDTO>> UpdateProduct(Guid id, ProductDTO productDto)
        {
            try
            {
                var existingProduct = await _repository.GetAsync(id);
                if (existingProduct == null)
                {
                    return Result<ProductDTO>.FailureResult("Product not found.", SharedKernel.Enums.ErrorCode.NotFound);
                }

                var product = _mapper.Map<Product>(productDto);
                product.UpdatedAt = DateTime.UtcNow;

                var productUpdated = await _repository.UpdateAsync(product);
                var productDtoUpdated = _mapper.Map<ProductDTO>(productUpdated);
                return Result<ProductDTO>.SuccessResult(productDtoUpdated);
            }
            catch (Exception ex)
            {
                return Result<ProductDTO>.FailureResult("Failed updating product.", SharedKernel.Enums.ErrorCode.UnexpectedError, ex);
            }
        }

        public async Task<Result<bool>> DeleteProduct(Guid id)
        {
            try
            {
                var product = await _repository.GetAsync(id);
                if (product == null)
                {
                    return Result<bool>.FailureResult("Product not found.", SharedKernel.Enums.ErrorCode.NotFound);
                }

                await _repository.DeleteAsync(product);
                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult("Failed deleting product.", SharedKernel.Enums.ErrorCode.UnexpectedError, ex);
            }
        }
    }
}
