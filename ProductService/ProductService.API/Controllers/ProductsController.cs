using Microsoft.AspNetCore.Mvc;
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

            if (result.Success)
            {
                return Ok(result.Data);
            }
            else
            {
                switch (result.ErrorCode)
                {
                    case SharedKernel.Enums.ErrorCode.NotFound:
                        return NotFound();
                    case SharedKernel.Enums.ErrorCode.ValidationError:
                        return BadRequest(result.Message);
                    case SharedKernel.Enums.ErrorCode.Unauthorized:
                        return Unauthorized(result.Message);
                    case SharedKernel.Enums.ErrorCode.Conflict:
                        return Conflict(result.Message);
                    case SharedKernel.Enums.ErrorCode.UnexpectedError:
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
                }
            }
        }
    }
}
