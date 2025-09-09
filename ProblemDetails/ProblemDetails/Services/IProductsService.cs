using ProblemDetails.Models;

namespace ProblemDetails.Services;

public interface IProductsService
{
    Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
    Task<ProductDTO?> GetAProductAsync(int id);
}