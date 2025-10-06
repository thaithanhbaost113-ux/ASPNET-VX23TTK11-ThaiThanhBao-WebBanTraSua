using Project.Models;
using Microsoft.AspNetCore.Mvc;
using Project.Areas.Identity.Data;

namespace Project.Repository
{
    public interface IProductRepository
    {
        public Task<List<Product>> GetAllProductAsync();
        public Task<Product> GetProductByIdAsync(string id);
        public Task<List<Product>> GetProductByNameAsync(string NameProduct);
        public Task<List<ProductModel>> GetByCategoryAsync(string Category);
        public Task<string> AddProductAsync(Product model);
        public Task AddImage(Image model);
        public Task UpDateProductAsync(string id, ProductModel model);
        public Task DeleteProductAsync(string id);
        public Task ActiveProductAsync(string id);
        public Task ChageNumberSaleAsync(string id, int? number);

    }
}
