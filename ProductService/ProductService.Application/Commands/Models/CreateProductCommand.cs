using MediatR;
using ProductService.Application.DTOs;
using SharedKernel.Models;

namespace ProductService.Application.Commands.Models
{
    public class CreateProductCommand : IRequest<Result<ProductDTO>>
    {
        public ProductDTO Product { get; }

        public CreateProductCommand(ProductDTO product)
        {
            Product = product;
        }
    }
}
