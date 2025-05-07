using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsProject.Data;
using NewsProject.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace NewsProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var newsList = await _context.News
                .Include(n => n.Author)
                .Include(n => n.Category)
                .OrderByDescending(n => n.PublishDate)
                .ToListAsync();

            var sliders = await _context.Slider.ToListAsync();

            ViewBag.Sliders = sliders;

            return View(newsList);
        }


        public async Task<IActionResult> Details(int id)
        {
            var news = await _context.News
                .Include(n => n.Author)
                .Include(n => n.Category)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (news == null)
            {
                return NotFound();
            }

            // Yorumlar� ve her bir yorumun kullan�c� bilgilerini al
            var comments = await _context.Comments
                .Include(c => c.User)  // Yorum yapan kullan�c� bilgisi
                .Where(c => c.NewsId == id)
                .OrderByDescending(c => c.CommentDate)
                .ToListAsync();

            var model = new NewsDetailsViewModel
            {
                News = news,
                Comments = comments
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddComment(int newsId, string content)
        {
            // ModelState kontrol�
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Yorum i�eri�i bo� olamaz!";
                return RedirectToAction("Details", new { id = newsId });
            }

            try
            {
                // Kullan�c� bilgilerini al
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(userId, out int userIdInt))
                {
                    TempData["Error"] = "Kullan�c� bilgileri ge�ersiz!";
                    return RedirectToAction("Login", "Account");
                }

                // Haberin var oldu�undan emin ol
                var newsExists = await _context.News.AnyAsync(n => n.Id == newsId);
                if (!newsExists)
                {
                    TempData["Error"] = "Haber bulunamad�!";
                    return RedirectToAction("Index", "Home");
                }

                var comment = new Comment
                {
                    NewsId = newsId,
                    Content = content.Trim(),
                    UserId = userIdInt,
                    CommentDate = DateTime.Now
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Yorumunuz ba�ar�yla eklendi!";
            }
            catch (Exception ex)
            {
                // Hata loglama yap�labilir
                TempData["Error"] = "Yorum eklenirken bir hata olu�tu!";
            }

            return RedirectToAction("Details", new { id = newsId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId, int newsId)
        {
            try
            {
                // Kullan�c� bilgilerini al
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var isAdmin = User.IsInRole("Admin");

                // Yorumu bul
                var comment = await _context.Comments
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == commentId);

                if (comment == null)
                {
                    TempData["Error"] = "Yorum bulunamad�!";
                    return RedirectToAction("Details", new { id = newsId });
                }

                // Silme yetkisi kontrol�
                if (!comment.CanBeDeletedBy(userId, isAdmin))
                {
                    TempData["Error"] = "Bu i�lem i�in yetkiniz yok!";
                    return RedirectToAction("Details", new { id = newsId });
                }

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Yorum ba�ar�yla silindi!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Yorum silinirken bir hata olu�tu!";
                // Loglama yapabilirsiniz: _logger.LogError(ex, "Yorum silinemedi");
            }

            return RedirectToAction("Details", new { id = newsId });
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet] // [HttpPost] yerine [HttpGet] yapt�k
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}