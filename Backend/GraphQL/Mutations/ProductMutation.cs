
using Backend.Data;
using Backend.Model;
using Backend.Service;
using Microsoft.EntityFrameworkCore;
namespace Backend.GraphQL.Mutations
{
   [ExtendObjectType("Mutation")]
    public class ProductMutation
    {
        
        public async Task<Products> CreateProductAsync(
           string name, string description, double price, string category, string image, bool inStock, int stockQuantity, string unit,
            [Service] ApplicationDbContext dbContext,
            [Service] IProductService productService)
        {
            
               return await productService.CreateProductAsync(name, description, price, category, image, inStock, stockQuantity, unit);
        }
        public async Task<Products> DeleteProductAsync(
           int productId,
            [Service] IProductService productService)
        {
               return await productService.DeleteProductAsync(productId);
        }
        public async Task<Products> UpdateProductStockAsync(
            int productId,
            int newStockQuantity,
            [Service] IProductService productService)
        {
            return await productService.UpdateProductStockAsync(productId, newStockQuantity);
        }
        public async Task<bool> ReserveProductStockAsync(
            int productId,
            int quantity,
            [Service] IProductService productService)
        {
            return await productService.ReserveProductStockAsync(productId, quantity);
        }
        public async Task<Products>AddPromotionToProductAsync(
            int productId,
            double promotionPrice,
            DateTime promotionStartDate,
            DateTime promotionEndDate,
            [Service] IProductService productService)
        {
            return await productService.AddPromotionToProductAsync(productId, promotionPrice, promotionStartDate, promotionEndDate);
        }
        
    }
}