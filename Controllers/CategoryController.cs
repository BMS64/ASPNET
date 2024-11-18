using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarketplace.Data;
using OnlineMarketplace.Models;

namespace OnlineMarketplace.Controllers;

[Authorize(Roles = UserRoles.Admin)]
public class CategoryController : Controller
{
    private readonly ShopContext _context;

    public CategoryController(ShopContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index() => View(await _context.Categories.ToListAsync());

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return View("NotFound");

        var category = await _context.Categories
            .FirstOrDefaultAsync(m => m.Id == id);
        if (category == null)
            return View("NotFound");

        return View(category);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Description")] CreateCategoryVM category)
    {
        if (ModelState.IsValid)
        {
            _context.Add(new Category() { Name = category.Name, Description = category.Description });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return View("NotFound");


        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return View("NotFound");

        return View(new EditeCategoryVM() { Name = category.Name, Description = category.Description });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] EditeCategoryVM category)
    {
        if (id != category.Id)
            return View("NotFound");

        if (ModelState.IsValid)
        {
            try {
                _context.Update(new Category() { Id = category.Id, Name = category.Name, Description = category.Description });
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!CategoryExists(category.Id))
                    return View("NotFound");
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return View("NotFound");

        var category = await _context.Categories
            .FirstOrDefaultAsync(m => m.Id == id);
        if (category == null)
            return View("NotFound");

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
            _context.Categories.Remove(category);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}
