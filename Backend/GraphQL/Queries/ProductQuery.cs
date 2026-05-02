
using Backend.Data;
using Backend.Model;
using Backend.Service;
using Microsoft.EntityFrameworkCore;
namespace Backend.GraphQL.Queries
{
    [ExtendObjectType("Query")]
    public class ProductQuery
    {
        public async Task<List<Product>>GetProductsByCategoryAsync(
            string category, 
            [Service] IProductService productService)
        {
            return await productService.GetProductsByCategoryAsync(category);
        }
     
        public async Task<List<Product>>AllProductsAsync(
            [Service] IProductService productService)
        {
            return  await  productService.AllProductsAsync();
        }

        public async Task<Product?> ProductByIdAsync(
            int id,
            [Service] IProductService productService)
        {
            return await productService.GetProductByIdAsync(id);
        }
        public async Task<List<Product>> GetPromotionProductsAsync(
            [Service] IProductService productService)
        {
            return await productService.GetPromotionProductsAsync();
        }
    }
}