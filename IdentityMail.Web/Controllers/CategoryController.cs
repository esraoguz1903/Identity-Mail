using IdentityMail.Web.Context;
using IdentityMail.Web.DTOs.CategoryDtos;
using IdentityMail.Web.Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace IdentityMail.Web.Controllers
{
    [Authorize(Roles ="User")]
    public class CategoryController(AppDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            var dtoCategories = categories.Adapt<List<ListCategoryDto>>();
            return View(dtoCategories);
        }
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            var category = createCategoryDto.Adapt<Category>();
             _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            var categoryDto = category.Adapt<UpdateCategoryDto>();
            return View(categoryDto);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto updateCategoryDto)
        {
            
            var category = await _context.Categories.FindAsync(updateCategoryDto.Id);

            if (category == null) return NotFound();
            
            updateCategoryDto.Adapt(category);
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
