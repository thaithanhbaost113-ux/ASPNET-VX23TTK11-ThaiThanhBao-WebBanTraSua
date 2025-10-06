using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Areas.Identity.Data;
using Project.Repository;
using Project.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

namespace Project.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[Controller]/[action]")]
    public class AdminController : Controller
    {
        private readonly IBillRepository _billRepo;
        private readonly IProductRepository _ProductRepo;
        private readonly IAccountRepository _accRepo;
        private readonly DataContext _DbContext;
        private Random _random = new Random();
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(IAccountRepository accRepo, 
            IBillRepository billRepo, 
            IProductRepository ProductRepo, 
            DataContext DbContext, 
            Random random, 
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _ProductRepo = ProductRepo;
            _billRepo = billRepo;
            _accRepo = accRepo;
            _DbContext = DbContext;
            _random = random;
            _userManager = userManager;
            _roleManager = roleManager;
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
        public async Task<IActionResult> BillControl()
        {
            return View(await _billRepo.GetAllBill());
        }
        
        public async Task<IActionResult> OrderBillControl(DateTime startDate, DateTime endDate)
        {
            var bill = await _DbContext.Bills
                .Where(x => x.BuyingDate >= startDate && x.BuyingDate < endDate).Include(x => x.User).Include(x => x.BillItems)
                .ToListAsync();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(bill);
        }
        public async Task<IActionResult> ProductControl(int page = 1)
        {
            int pageSize = 10;
            var totalProducts = await _DbContext.Products.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var products = await _DbContext.Products
                .Include(x => x.Images)
                .Include(x => x.ProductCategory)
                .Skip((page - 1) * pageSize) 
                .Take(pageSize) 
                .ToListAsync();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(products);
        }

        public async Task<IActionResult> AccControl()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            ViewBag.Roles = roles;

            var users = await _accRepo.ShowUserAsync();
            var userRoles = new Dictionary<string, List<string>>();

            foreach (var user in users)
            {
                var userRoleList = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = userRoleList.ToList();
            }
            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatusProduct(string Id, int status)
        {
            var product = _DbContext.Products.FirstOrDefault(x => x.Id == Id);
            if (product != null)
            {
                product.Status = status;
                _DbContext.Products.Update(product);
                await _DbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ProductControl)); 
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMultipleUserRoles([FromBody] List<UpdateUserRoleModel> models)
        {
            if (models == null || !models.Any())
                return BadRequest("No data provided.");

            foreach (var model in models)
            {
                if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.SelectedRole))
                    return BadRequest("Invalid data.");

                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                    return NotFound($"User with ID {model.UserId} not found.");

                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                var result = await _userManager.AddToRoleAsync(user, model.SelectedRole);
                if (!result.Succeeded)
                    return BadRequest($"Failed to update role for user {user.UserName}.");
            }

            return Ok("Roles updated successfully.");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.SelectedRole))
                return BadRequest("Invalid data.");

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound("User not found.");

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles); 
            var result = await _userManager.AddToRoleAsync(user, model.SelectedRole); 

            if (result.Succeeded)
                return Ok();

            return BadRequest("Failed to update role.");
        }

        public async Task<IActionResult> GetUserByEmail(string email)
        {
            await _accRepo.GetUesrByEmailAsync(email);
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ChangeStatusBill(string id, int status)
        {
            string email = HttpContext.User.Identity.Name;
            await _billRepo.UpdateBillAsync(id, status);
            return RedirectToAction(nameof(BillControl));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(string IdProduct)
        {
            await _ProductRepo.DeleteProductAsync(IdProduct);
            return RedirectToAction(nameof(ProductControl));
        }

        [HttpGet]
        public async Task<IActionResult> ActiveProduct(string IdProduct)
        {
            await _ProductRepo.ActiveProductAsync(IdProduct);
            return RedirectToAction(nameof(ProductControl));
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(string IdProduct)
        {
            var product = await _ProductRepo.GetProductByIdAsync(IdProduct);
            if (product == null)
            {
                return NotFound();
            }
            var categories = _DbContext.ProductCategorys
                .Where(x => x.Status == true)
                .Select(c => new SelectListItem
                {
                    Value = c.Id,
                    Text = c.Name,
                    Selected = c.Id == product.IdCategory 
                }).ToList();

            ViewBag.Categories = categories;
            var productModel = new ProductModel
            {
                Id = product.Id,
                NameProduct = product.NameProduct,
                IdCategory = product.IdCategory,
                Price = product.Price,
                Review = product.Review,
                Picture = product.Images
                    .Where(img => img.IsMain == true)
                    .FirstOrDefault()?.UrlImage 
            };

            return View(productModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductModel model, IFormFile Picture)
        {
            var product = await _ProductRepo.GetProductByIdAsync(model.Id.ToString());
            if (product == null)
            {
                return NotFound();
            }

            product.IdCategory = model.IdCategory;
            product.NameProduct = model.NameProduct;
            product.Price = model.Price;
            product.Review = model.Review;

            if (Picture != null && Picture.Length > 0)
            {
                var fileName = Path.GetFileName(Picture.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Image", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Picture.CopyToAsync(fileStream);
                }

                var oldMainImage = product.Images.FirstOrDefault(img => img.IsMain == true);
                if (oldMainImage != null)
                {
                    _DbContext.Images.Remove(oldMainImage);
                }

                var newPicture = new Image
                {
                    UrlImage = "/Image/" + fileName,
                    IsMain = true,
                    IdProduct = product.Id,
                };
                await _ProductRepo.AddImage(newPicture);
            }

            await _DbContext.SaveChangesAsync();

            return RedirectToAction(nameof(ProductControl));
        }


        public IActionResult AddProduct()
        {
            var categories = _DbContext.ProductCategorys.Where(x => x.Status == true)
               .Select(c => new SelectListItem
               {
                   Value = c.Id,
                   Text = c.Name
               }).ToList();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(string NameProduct, IFormFile Picture, string IdCategory, int Price, string Review)
        {
            if (Picture != null && Picture.Length > 0)
            {
                var fileName = Path.GetFileName(Picture.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Image", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Picture.CopyToAsync(fileStream);
                }
                Product model = new Product()
                {
                    Number = 0,
                    IdCategory = IdCategory,
                    NameProduct = NameProduct,
                    Price = Price,
                    Sales = 0,
                    Status = 1,
                    Review = Review,
                };
                var NewProduct = await _ProductRepo.AddProductAsync(model);
                var Pictures = new Image()
                {
                    UrlImage = "/Image/" + fileName,
                    IsMain = true,
                    IdProduct = model.Id,
                };

                await _ProductRepo.AddImage(Pictures);
                var Product = await _ProductRepo.GetProductByIdAsync(NewProduct);
                return RedirectToAction(nameof(ProductControl));
            }
            ModelState.AddModelError("Picture", "Please upload a valid image.");
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> BlogControl()
        {
            var blog = await _DbContext.Blogs.ToListAsync();
            return View(blog);
        }

        [HttpGet]
        public async Task<ActionResult> GetBlogByNameControl(string name)
        {
            var blog = await _DbContext.Blogs.Where(x => x.Title.Contains(name)).ToListAsync();
            return View(blog);
        }
        [HttpGet]
        public async Task<ActionResult> AddBlog()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddBlog(BlogModel model)
        {
            var blog = new Blog
            {
                Id = RandomId(),
                Title = model.Title,
                Conten = model.Conten,
                Date = DateTime.Now
            };
            if (model.ImageFile != null)
            {
                var fileName = Path.GetFileName(model.ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Image", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                blog.Image = $"/Image/{fileName}";
            }

            _DbContext.Blogs.Add(blog);
            await _DbContext.SaveChangesAsync();

            return RedirectToAction("BlogControl");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteBlog(string Id)
        {
            var blog = await _DbContext.Blogs.FirstOrDefaultAsync(x => x.Id == Id);
            if (blog == null)
            {
                return NotFound();
            }
            _DbContext.Remove(blog);
            _DbContext.SaveChanges();
            return RedirectToAction("BlogControl");
        }
        [HttpGet]
        public async Task<IActionResult> EditBlog(string id)
        {
            var blog = await _DbContext.Blogs.FirstOrDefaultAsync(x => x.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            var model = new BlogModel
            {
                Id = blog.Id,
                Title = blog.Title,
                Conten = blog.Conten,
                Date = blog.Date
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBlog(BlogModel model)
        {
            var blog = await _DbContext.Blogs.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (blog == null)
            {
                return NotFound();
            }

            blog.Title = model.Title;
            blog.Conten = model.Conten;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Image", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                blog.Image = "/Image/" + fileName;
            }

            _DbContext.Blogs.Update(blog);
            await _DbContext.SaveChangesAsync();

            return RedirectToAction(nameof(BlogControl));
        }



        [HttpGet]
        public async Task<ActionResult> ReceiveControl()
        {
            var receives = await _DbContext.Receives
                .Include(db => db.ProviderNavigation)
                .Include(db => db.RecevieDetails)
                .ThenInclude(db => db.IdProductNavigation)
                .ToListAsync();
            return View(receives);
        }

        public async Task<ActionResult> AddReceive()
        {
            ViewBag.Products = await _DbContext.Products.ToListAsync();
            ViewBag.Providers = await _DbContext.Providers.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddReceive(Receive receive)
        {
            receive.Id = RandomId();
            _DbContext.Add(receive);
            await _DbContext.SaveChangesAsync();

            foreach (var detail in receive.RecevieDetails)
            {
                detail.Id = RandomId();
                detail.IdReceive = receive.Id;
                _DbContext.Add(detail);
            }

            await _DbContext.SaveChangesAsync();
            return RedirectToAction("ReceiveControl");
        }
        
        public async Task<IActionResult> ProductCategoryControl()
        {
            return _DbContext.ProductCategorys != null ?
                        View(await _DbContext.ProductCategorys.ToListAsync()) :
                        Problem("Entity set 'DataContext.ProductCategorys'  is null.");
        }


        public async Task<IActionResult> EditProductCategory(string id)
        {
            if (id == null || _DbContext.ProductCategorys == null)
            {
                return NotFound();
            }

            var productCategory = await _DbContext.ProductCategorys.FindAsync(id);
            if (productCategory == null)
            {
                return NotFound();
            }
            return View(productCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProductCategory(string id, [Bind("Id,Name,Status")] ProductCategory productCategory)
        {
            if (id != productCategory.Id)
            {
                return NotFound();
            }

            _DbContext.Update(productCategory);
            await _DbContext.SaveChangesAsync();
            return RedirectToAction(nameof(ProductCategoryControl));
        }


        public IActionResult CreateProductCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProductCategory([Bind("Name,Status")] ProductCategory productCategory)
        {
            productCategory.Id = RandomId();
            _DbContext.Add(productCategory);
            await _DbContext.SaveChangesAsync();
            return RedirectToAction(nameof(ProductCategoryControl));
        }
    }
}
