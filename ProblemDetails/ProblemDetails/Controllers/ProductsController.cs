using Microsoft.AspNetCore.Mvc;
using ProblemDetails.Models;
using ProblemDetails.Services;

namespace ProblemDetails.Controllers;

[Route("[controller]")]
[ApiController]
public class ProductsController(IProductsService productsService, ILogger<ProductsController> logger) : ControllerBase
{
    // GET: /Products
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductDTO>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts()
    {
        logger.LogInformation("Retrieving all products");

        try
        {
            var products = await productsService.GetAllProductsAsync();

            if (!products.Any())
                return NoContent();

            return Ok(products);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while retrieving all products");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    // GET: /products/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Microsoft.AspNetCore.Mvc.ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Microsoft.AspNetCore.Mvc.ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError,
        Type = typeof(Microsoft.AspNetCore.Mvc.ProblemDetails))]
    public async Task<ActionResult<ProductDTO>> GetAProduct(int id)
    {
        logger.LogInformation($"Retrieving product with id {id}");

        try
        {
            var product = await productsService.GetAProductAsync(id);

            if (product == null)
                return Problem(
                    $"Product with ID {id} was not found.",
                    title: "Product not found",
                    statusCode: StatusCodes.Status404NotFound,
                    instance: HttpContext.TraceIdentifier
                );

            return Ok(product);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogError(ex, "Unauthorized access");
            return Problem(
                ex.Message,
                title: "Unauthorized Access",
                statusCode: StatusCodes.Status401Unauthorized,
                instance: HttpContext.TraceIdentifier
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred while retrieving product with id {id}");
            return Problem(
                "An unexpected error occurred while processing your request.",
                title: "Internal Server Error",
                statusCode: StatusCodes.Status500InternalServerError,
                instance: HttpContext.TraceIdentifier
            );
        }
    }
}