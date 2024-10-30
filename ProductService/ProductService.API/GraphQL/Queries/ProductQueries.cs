using HotChocolate.Language;
using HotChocolate.Resolvers;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Application.Queries.Models;
using ProductService.Infrastructure.Data.Context;

namespace ProductService.API.GraphQL.Queries
{
    public class ProductQueries
    {
        [GraphQLName("getProductById")]
        public async Task<ProductDTO?> GetProductById([Service] IMediator mediator, Guid id)
        {
            var result = await mediator.Send(new GetProductByIdQuery(id));
            return result.Success ? result.Data : null;
        }

        //[UseFiltering, UseSorting]
        //[GraphQLName("getProducts")]
        //public async Task<IEnumerable<ProductDTO>> GetProducts([Service] IMediator mediator)
        //{
        //    var result = await mediator.Send(new GetProductsQuery());
        //    return result.Success ? result.Data : null!;
        //}

        //[UseFiltering, UseSorting]
        //[GraphQLName("getProducts")]
        //public IQueryable<ProductDTO> GetProducts([Service] ProductDbContext dbContext, [Service] IMapper mapper)
        //{
        //    // Fetches all fields from db and returns (projects) only selected ones.
        //    return dbContext.Products.ProjectTo<ProductDTO>(mapper.ConfigurationProvider);
        //}

        // More performant way. Fetches only requested fields from db.
        [UseFiltering, UseSorting]
        [GraphQLName("getProducts")]
        public IQueryable<ProductDTO> GetProducts([Service] ProductDbContext dbContext, IResolverContext context)
        {
            // Get the fields requested by the GraphQL query
            var selections = context.Selection.SyntaxNode.SelectionSet?.Selections.OfType<FieldNode>();

            var selectedFields = selections?.Select(x => x.Name.Value).ToList();

            var query = dbContext.Products.AsQueryable();

            if (selectedFields == null || !selectedFields.Any())
            {
                return query.Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    SKU = p.SKU,
                    Category = p.Category,
                    ImageUrl = p.ImageUrl,
                    CreatedAt = p.CreatedAt
                });
            }

            // Dynamic projection
            return query.Select(p => new ProductDTO
            {
                Id = selectedFields.Contains("id") ? p.Id : Guid.Empty,
                Name = selectedFields.Contains("name") ? p.Name : null!,
                Description = selectedFields.Contains("description") ? p.Description : null!,
                Price = selectedFields.Contains("price") ? p.Price : 0,
                SKU = selectedFields.Contains("sku") ? p.SKU : null!,
                Category = selectedFields.Contains("category") ? p.Category : null!,
                ImageUrl = selectedFields.Contains("imageUrl") ? p.ImageUrl : null!,
                CreatedAt = selectedFields.Contains("createdAt") ? p.CreatedAt : null
            });
        }
    }
}
