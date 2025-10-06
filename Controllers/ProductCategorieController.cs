using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Areas.Identity.Data;

namespace Project.Controllers
{
    public class ProductCategorieController : Controller
    {
        private readonly DataContext _context;

        public ProductCategorieController(DataContext context)
        {
            _context = context;
        }

        // GET: ProductCategorie
        public async Task<IActionResult> Index()
        {
              return _context.ProductCategorys != null ? 
                          View(await _context.ProductCategorys.ToListAsync()) :
                          Problem("Entity set 'DataContext.ProductCategorys'  is null.");
        }

        // GET: ProductCategorie/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.ProductCategorys == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategorys
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            return View(productCategory);
        }

        // GET: ProductCategorie/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductCategorie/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Status")] ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productCategory);
        }

        // GET: ProductCategorie/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.ProductCategorys == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategorys.FindAsync(id);
            if (productCategory == null)
            {
                return NotFound();
            }
            return View(productCategory);
        }

        // POST: ProductCategorie/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Status")] ProductCategory productCategory)
        {
            if (id != productCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductCategoryExists(productCategory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productCategory);
        }

        // GET: ProductCategorie/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.ProductCategorys == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategorys
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            return View(productCategory);
        }

        // POST: ProductCategorie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.ProductCategorys == null)
            {
                return Problem("Entity set 'DataContext.ProductCategorys'  is null.");
            }
            var productCategory = await _context.ProductCategorys.FindAsync(id);
            if (productCategory != null)
            {
                _context.ProductCategorys.Remove(productCategory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductCategoryExists(string id)
        {
          return (_context.ProductCategorys?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
