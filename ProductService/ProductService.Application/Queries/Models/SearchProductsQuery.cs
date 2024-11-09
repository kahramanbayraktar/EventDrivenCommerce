using MediatR;
using ProductService.Application.DTOs;
using SharedKernel.Models;

namespace ProductService.Application.Queries.Models
{
    public class SearchProductsQuery : IRequest<Result<IEnumerable<ProductDTO>>>
    {
        public string Keyword { get; }

        public SearchProductsQuery(string keyword)
        {
            Keyword = keyword;
        }
    }
}
