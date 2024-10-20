using Microsoft.AspNetCore.Mvc;
using ProductService.API.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using SharedKernel.Enums;
using SharedKernel.Models;

namespace ProductService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var result = await _service.GetProductById(id);

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
            var result = await _service.CreateProduct(product);

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
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductDTO product)
        {
            var result = await _service.UpdateProduct(id, product);

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
            var result = await _service.DeleteProduct(id);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return HandleError(result);
            }
        }

        private IActionResult HandleError<T>(Result<T> result)
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
