using Microsoft.EntityFrameworkCore;
using OnlineMarketplace.Data.Repository;
using OnlineMarketplace.Models;

namespace OnlineMarketplace.Data.Services;

public class ProductService : EntityBaseRepository<Product>, IProductService
{
    private readonly ShopContext context;

    public ProductService(ShopContext context) : base(context)
    {
        this.context = context;
    }

    public async Task<List<Product>> GetProductsByIdsAsync(List<int> productIds)
    {
        return await context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
    }

}
