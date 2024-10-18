using ProductService.Application.DTOs;
using SharedKernel.Models;

namespace ProductService.Application.Interfaces
{
    public interface IProductService
    {
        Task<Result<ProductDTO>> GetProductById(Guid id);
    }
}
