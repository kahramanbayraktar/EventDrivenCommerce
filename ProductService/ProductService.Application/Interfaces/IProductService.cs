using ProductService.Application.DTOs;
using SharedKernel.Models;

namespace ProductService.Application.Interfaces
{
    public interface IProductService
    {
        Task<Result<ProductDTO>> GetProductById(Guid id);
        Task<Result<ProductDTO>> CreateProduct(ProductDTO productDTO);
        Task<Result<ProductDTO>> UpdateProduct(Guid id, ProductDTO productDTO);
        Task<Result<bool>> DeleteProduct(Guid id);
    }
}
