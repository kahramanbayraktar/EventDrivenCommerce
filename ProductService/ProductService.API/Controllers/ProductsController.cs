using Microsoft.AspNetCore.Mvc;
using ProductService.API.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;

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

            var response = new ApiResponse<ProductDTO>
            {
                Data = result.Data,
                Success = result.Success,
                Message = result.Message,
                TraceId = Guid.NewGuid().ToString()
            };

            if (result.Success)
            {
                return Ok(response);
            }
            else
            {
                return result.ErrorCode switch
                {
                    SharedKernel.Enums.ErrorCode.NotFound => NotFound(),
                    SharedKernel.Enums.ErrorCode.ValidationError => BadRequest(result.Message),
                    SharedKernel.Enums.ErrorCode.Unauthorized => Unauthorized(result.Message),
                    SharedKernel.Enums.ErrorCode.Conflict => Conflict(result.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Message),
                };
            }
        }
    }
}
