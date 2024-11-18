using OnlineMarketplace.Data.Repository;
using OnlineMarketplace.Models;

namespace OnlineMarketplace.Data.Services;

public interface IProductService : IEntityBaseRepository<Product>
{
    Task<List<Product>> GetProductsByIdsAsync(List<int> productIds);
}