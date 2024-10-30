using MediatR;
using ProductService.Application.DTOs;
using SharedKernel.Models;

namespace ProductService.Application.Queries.Models
{
    public class GetProductsQuery : IRequest<Result<IEnumerable<ProductDTO>>>
    {
    }
}
