using MediatR;
using ProductService.Application.Commands.Models;
using ProductService.Application.DTOs;

namespace ProductService.API.GraphQL.Mutations
{
    public class ProductMutations
    {
        public async Task<ProductDTO?> CreateProduct(IMediator mediator, ProductDTO product)
        {
            var result = await mediator.Send(new CreateProductCommand(product));
            return result.Success ? result.Data : null;
        }

        public async Task<ProductDTO?> UpdateProduct(IMediator mediator, ProductDTO product)
        {
            var result = await mediator.Send(new UpdateProductCommand(product));
            return result.Success ? result.Data : null;
        }
    }
}
