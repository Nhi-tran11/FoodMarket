using Backend.Data;
using Backend.GraphQL.Mutations;
using Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service;
public interface IProductService
{
    Task<List<Products>> GetProductsByCategoryAsync(string category);
    Task<Products> CreateProductAsync(
           string name, string description, double price, string category, string image, bool inStock, int stockQuantity, string unit);
    Task<List<Products>> AllProductsAsync();
    Task<Products?> GetProductByIdAsync(int id);
    Task<Products> DeleteProductAsync(int productId);
    Task<Products> UpdateProductStockAsync(int productId, int newStockQuantity);
    Task<bool>ReserveProductStockAsync(int productId, int quantity); 
    Task<Products> AddPromotionToProductAsync(int productId, double promotionPrice, DateTime promotionStartDate, DateTime promotionEndDate);   
    Task<List<Products>> GetPromotionProductsAsync();
}
public sealed class ProductService : IProductService
{
    public readonly ApplicationDbContext _dbContext;
    public ProductService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Products>> AllProductsAsync()
    {
        return _dbContext.Products.ToListAsync();
    }

    public async Task<Products?> GetProductByIdAsync(int id)
    {
        return await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public Task<Products> CreateProductAsync(string name, string description, double price, string category, string image, bool inStock, int stockQuantity, string unit)
    {
        var existingProduct = _dbContext.Products.FirstOrDefault(p => p.Name ==name);
        if(existingProduct !=null)
        {
            throw new Exception("Product with the same name already exists.");
        }
        var newProduct = new Products
        {
            Name = name,
            Description = description,
            Price = price,
            Category = category,
            Image = image,
            InStock = inStock,
            StockQuantity = stockQuantity,
            Unit = unit
        };
        _dbContext.Products.Add(newProduct);
        _dbContext.SaveChanges();
        return Task.FromResult(newProduct);
    }

    public Task<Products> DeleteProductAsync(int productId)
    {
        var product = _dbContext.Products.FirstOrDefault(p => p.Id == productId);
        if(product == null)
        {
            throw new Exception("Product not found.");
        }
        _dbContext.Products.Remove(product);
        _dbContext.SaveChanges();
        return Task.FromResult(product);
    }

    public Task<List<Products>> GetProductsByCategoryAsync(string category)
    {
        return _dbContext.Products
                .Where(p => p.Category.ToLower() == category.ToLower())
                .ToListAsync();
    }

    public async Task<bool> ReserveProductStockAsync(int productId, int quantity)
    {
        const int maxRetryCount =3;
        int retryCount =0;
        while(retryCount < maxRetryCount)
        {
            try{
            var product = _dbContext.Products.FirstOrDefault(p=>p.Id == productId);
            if(product == null)
            {
                throw new Exception("Product is not found.");
            }
            if(product.StockQuantity <quantity)
            {
                return false;
            }
            product.StockQuantity -= quantity;
            product.InStock = product.StockQuantity >0;

                await _dbContext.SaveChangesAsync();
                return true;
 
        }
            catch(DbUpdateConcurrencyException)
            {
                //Someone else has updated the product, retry
                retryCount++;
                //Reload the entity from the database
                foreach(var entry in _dbContext.ChangeTracker.Entries())
                {
                    entry.Reload();
                }
                if(retryCount == maxRetryCount)
                {
                    throw new Exception("Could not reserve product stock due to concurrent updates. Please try again.");
                }
            }

    }
    return false;
    }

    public Task<Products> UpdateProductStockAsync(int productId, int newStockQuantity)
    {
        var product = _dbContext.Products.FirstOrDefault(p =>p.Id == productId);
        if(product == null)
        {
            throw new Exception("Product is not found.");
        }
        else
        {
            product.StockQuantity = newStockQuantity;
            product.InStock = newStockQuantity>0;
            _dbContext.Products.Update(product);
            _dbContext.SaveChanges();
            return Task.FromResult(product);
        }
    }

    public Task<Products> AddPromotionToProductAsync(int productId, double promotionPrice, DateTime promotionStartDate, DateTime promotionEndDate)
    {
       var product = _dbContext.Products.FirstOrDefault(p=>p.Id == productId);
       if(product == null)
       {
         throw new Exception ("Product is not found.");
       }
         if(promotionPrice >= product.Price)
         {
          throw new Exception("Promotion price must be less than the original price.");
         }
            if(promotionStartDate >= promotionEndDate)
            {
                throw new Exception("Promotion start date must be before end date.");
            }
            product.PromotionPrice = promotionPrice;
            product.PromotionStartDate = promotionStartDate;
            product.PromotionEndDate = promotionEndDate;
            _dbContext.Products.Update(product);
            _dbContext.SaveChanges();
            return Task.FromResult(product);

    }

    public async Task<List<Products>> GetPromotionProductsAsync()
    {
        var now = DateTime.UtcNow;
    
        // Now get only active promotions
        var activePromotions = await _dbContext.Products
            .Where(p => p.PromotionPrice != null 
                && p.PromotionStartDate != null 
                && p.PromotionEndDate != null 
                && p.PromotionStartDate.Value <= now 
                && p.PromotionEndDate.Value >= now)
            .ToListAsync();
        
        return activePromotions;
    }
}