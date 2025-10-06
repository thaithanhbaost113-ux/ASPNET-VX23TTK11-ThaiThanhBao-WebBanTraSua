using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Areas.Identity.Data;
using System.Xml.Linq;

namespace Project.Controllers
{
    public class BlogController : Controller
    {
        public readonly DataContext _DbContext;
        public BlogController(DataContext dataContext)
        {
            _DbContext = dataContext;
        }
        [HttpGet]
        public async Task<ActionResult> GetAllBlog()
        {
            var blog = await _DbContext.Blogs.ToListAsync();
            return View(blog);
        }

        [HttpGet]
        public async Task<ActionResult> GetBlog(string Id)
        {
            var blog = await _DbContext.Blogs.FirstOrDefaultAsync(x => x.Id == Id);
            return View(blog);
        }
        [HttpGet]
        public async Task<ActionResult> GetBlogs(string name)
        {
            var blog = await _DbContext.Blogs.Where(x => x.Title.Contains(name)).ToListAsync();
            return View(blog);
        }
    }
}
