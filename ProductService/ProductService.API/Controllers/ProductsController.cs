using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.API.Models;
using ProductService.Application.Commands.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Queries.Models;
using SharedKernel.Enums;

namespace ProductService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(id));

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return HandleError(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO product)
        {
            var result = await _mediator.Send(new CreateProductCommand(product));

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return HandleError(result);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductDTO product)
        {
            var result = await _mediator.Send(new UpdateProductCommand(product));

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return HandleError(result);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id));

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return HandleError(result);
            }
        }

        private IActionResult HandleError<T>(SharedKernel.Models.Result<T> result)
        {
            var response = new ApiResponse<T>
            {
                Data = result.Data,
                Success = result.Success,
                Message = result.Message,
                TraceId = Guid.NewGuid().ToString()
            };

            return result.ErrorCode switch
            {
                ErrorCode.NotFound => NotFound(),
                ErrorCode.ValidationError => BadRequest(response),
                ErrorCode.Unauthorized => Unauthorized(response),
                ErrorCode.Conflict => Conflict(response),
                _ => StatusCode(StatusCodes.Status500InternalServerError, response),
            };
        }
    }
}
