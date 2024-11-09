using AutoMapper;
using Elastic.Clients.Elasticsearch;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Application.Queries.Models;
using SharedKernel.Enums;
using SharedKernel.Models;

namespace ProductService.Application.Queries.Handlers
{
    public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, Result<IEnumerable<ProductDTO>>>
    {
        private readonly ElasticsearchClient _elasticClient;
        private readonly IMapper _mapper;

        public SearchProductsQueryHandler(ElasticsearchClient elasticClient, IMapper mapper)
        {
            _elasticClient = elasticClient;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<ProductDTO>>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var searchResponse = await _elasticClient.SearchAsync<ProductDTO>(s => s
                    .Query(q => q
                        .MultiMatch(m => m
                            .Query(request.Keyword)
                            .Fields(new[] { "name", "description" })
                        )
                    )
                );

                if (!searchResponse.IsValidResponse)
                {
                    return Result<IEnumerable<ProductDTO>>.FailureResult("Failed searching products in Elasticsearch.", ErrorCode.UnexpectedError);
                }

                var products = _mapper.Map<IEnumerable<ProductDTO>>(searchResponse.Documents);
                return Result<IEnumerable<ProductDTO>>.SuccessResult(products);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<ProductDTO>>.FailureResult("Failed searching products in Elasticsearch.", ErrorCode.UnexpectedError, ex);
            }
        }
    }
}
