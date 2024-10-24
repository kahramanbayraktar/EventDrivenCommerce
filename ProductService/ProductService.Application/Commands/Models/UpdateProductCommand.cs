using MediatR;
using ProductService.Application.DTOs;
using SharedKernel.Models;

namespace ProductService.Application.Commands.Models
{
    public class UpdateProductCommand : IRequest<Result<ProductDTO>>
    {
        public ProductDTO Product { get; }

        public UpdateProductCommand(ProductDTO product)
        {
            Product = product;
        }
    }
}
