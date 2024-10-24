using MediatR;
using ProductService.Application.DTOs;
using SharedKernel.Models;

namespace ProductService.Application.Queries.Models
{
    public class GetProductByIdQuery : IRequest<Result<ProductDTO>>
    {
        public Guid Id { get; }

        public GetProductByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
