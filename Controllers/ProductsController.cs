using Project.Areas.Identity.Data;
using Project.Models;
using Project.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data.SqlTypes;

namespace Project.Controllers
{
    //[Route("[Controller]/[action]")]
    public class ProductsController : Controller
    {
        public readonly IProductRepository _ProductRepo;
        private readonly DataContext _context;
        public ProductsController(IProductRepository ProductRepo, DataContext context)
        {
            _ProductRepo = ProductRepo;
            _context = context;
        }
        [HttpGet]
        public IActionResult GetAll(int page = 1, string category = "")
        {
            int pageSize = 8;

            var categories = _context.ProductCategorys.ToList(); 
            var query = _context.Products.Include(x =>x.ProductCategory).Include(x =>x.Images).AsQueryable();
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.ProductCategory.Name.Contains(category));
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var products = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewData["Category"] = category;  
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Categories = categories; 

            return View(products);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllProduct(int page = 1, string category = "")
        {
            int pageSize = 8;

            var categories = _context.ProductCategorys.ToList();
            var query = _context.Products.Include(x => x.Images).Include(x => x.ProductCategory).AsQueryable();
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.ProductCategory.Name.Contains(category));
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var products = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewData["Category"] = category;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Categories = categories;

            return View(products);
        }
        public async Task<ActionResult> About()
        {
            return View();
        }
        
        public async Task<ActionResult> News()
        {
            return View();
        }

        public async Task<ActionResult> Blog()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetProductById(string id)
        {
            if (id != null)
            {
                var product = await _ProductRepo.GetProductByIdAsync(id);
                return View(product);
            }
            else
            {
                return BadRequest();
            }
        }
       

    }
}
