using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsProject.Data;

namespace NewsProject.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Politics()
        {
            var newsList = await _context.News
                .Include(n => n.Author)
                .Include(n => n.Category)
                .Where(n => n.Category.Name == "Siyaset")
                .OrderByDescending(n => n.PublishDate)
                .ToListAsync();

            return View( newsList);
        }

        public async Task<IActionResult> Health()
        {
            var newsList = await _context.News
                .Include(n => n.Author)
                .Include(n => n.Category)
                .Where(n => n.Category.Name == "Sağlık")
                .OrderByDescending(n => n.PublishDate)
                .ToListAsync();

            return View(newsList);
        }

        public async Task<IActionResult> Sports()
        {
            var newsList = await _context.News
                .Include(n => n.Author)
                .Include(n => n.Category)
                .Where(n => n.Category.Name == "Spor")
                .OrderByDescending(n => n.PublishDate)
                .ToListAsync();

            return View(newsList);
        }

        public async Task<IActionResult> Magazine()
        {
            var newsList = await _context.News
                .Include(n => n.Author)
                .Include(n => n.Category)
                .Where(n => n.Category.Name == "Magazin")
                .OrderByDescending(n => n.PublishDate)
                .ToListAsync();

            return View(newsList);
        }

        public async Task<IActionResult> Technology()
        {
            var newsList = await _context.News
                .Include(n => n.Author)
                .Include(n => n.Category)
                .Where(n => n.Category.Name == "Teknoloji")
                .OrderByDescending(n => n.PublishDate)
                .ToListAsync();

            return View(newsList);
        }

        public async Task<IActionResult> Economy()
        {
            var newsList = await _context.News
                .Include(n => n.Author)
                .Include(n => n.Category)
                .Where(n => n.Category.Name == "Ekonomi")
                .OrderByDescending(n => n.PublishDate)
                .ToListAsync();

            return View(newsList);
        }
    }
}
