using AutoMapper;
using Project.Areas.Identity.Data;
using Project.Models;
using Project.Repository;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System;

namespace Project.Repository
{
    public class ProductRepository : IProductRepository
    {
        public readonly IMapper _mapper;
        public readonly DataContext _context;
        private Random _random = new Random();
        public ProductRepository(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public string RandomId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder idBuilder = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                idBuilder.Append(chars[_random.Next(chars.Length)]);
            }
            return idBuilder.ToString();
        }
        public async Task<string> AddProductAsync(Product model)
        {
            model.Id = RandomId();
            await _context.Products!.AddAsync(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task AddImage(Image model)
        {
            model.Id = RandomId();
            await _context.Images!.AddAsync(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteProductAsync(string id)
        {
            var Product = _context.Products!.SingleOrDefault(b => b.Id == id);
            Product.Status = 0;
            if (Product != null)
            {
                await _context.SaveChangesAsync();
            }
        }
        public async Task ActiveProductAsync(string id)
        {
            var Product = _context.Products!.SingleOrDefault(b => b.Id == id);
            Product.Status = 1;
            if (Product != null)
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Product>> GetAllProductAsync()
        {
            var ListProduct = await _context.Products!.Where(x => x.Status == 1).Include(x => x.Images).Include(x => x.ProductCategory).ToListAsync();
            return ListProduct;
        }
        public async Task ChageNumberSaleAsync(string id, int? number)
        {
            var Product = _context.Products!.SingleOrDefault(x => x.Id == id);
            Product.Sales += number;
            //await _context.SaveChangesAsync();
        }
        public async Task<Product> GetProductByIdAsync(string id)
        {
            var Product = await _context.Products!.Include(db => db.Images).Include(db => db.ProductCategory).FirstOrDefaultAsync(b => b.Id == id);
			return _mapper.Map<Product>(Product);
        }
        public async Task<List<Product>> GetProductByNameAsync(string NameProduct)
        {
			var Product = await _context.Products.Include(db => db.Images)
                    .Where(b => (string.IsNullOrEmpty(NameProduct) || b.NameProduct.Contains(NameProduct)) && b.Status == 1)
                    .ToListAsync();
            return Product;
        }
        public async Task<List<ProductModel>> GetByCategoryAsync(string NameProduct)
        {
            var Product = await _context.Products!.Where(b => b.Category == NameProduct).ToListAsync();
            return _mapper.Map<List<ProductModel>>(Product);
        }
        public async Task UpDateProductAsync(string id, ProductModel model)
        {
            if (id == model.Id)
            {
                var updateProduct = _mapper.Map<Product>(model);
                _context.Products!.Update(updateProduct);
                await _context.SaveChangesAsync();
            }
        }

    }
}
