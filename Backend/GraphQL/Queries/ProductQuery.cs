
using Backend.Data;
using Backend.Model;
using Backend.Service;
using Microsoft.EntityFrameworkCore;
namespace Backend.GraphQL.Queries
{
    [ExtendObjectType("Query")]
    public class ProductQuery
    {
        public async Task<List<Products>>GetProductsByCategoryAsync(
            string category, 
            [Service] IProductService productService)
        {
            return await productService.GetProductsByCategoryAsync(category);
        }
     
        public async Task<List<Products>>AllProductsAsync(
            [Service] IProductService productService)
        {
            return  await  productService.AllProductsAsync();
        }

        public async Task<Products?> ProductByIdAsync(
            int id,
            [Service] IProductService productService)
        {
            return await productService.GetProductByIdAsync(id);
        }
        public async Task<List<Products>> GetPromotionProductsAsync(
            [Service] IProductService productService)
        {
            return await productService.GetPromotionProductsAsync();
        }
    }
}