using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsProject.Data;
using NewsProject.Models;
using NewsProject.Utils;

namespace NewsProject.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/News
        public async Task<IActionResult> Index()
        {
            var newsWithRelations = await _context.News
                .Include(n => n.Category)
                .Include(n => n.Author)
                .ToListAsync();

            return View(newsWithRelations);
        }


        // GET: Admin/News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: Admin/News/Create
        public IActionResult Create()
        {
            ViewBag.Authors = new SelectList(_context.Authors, "Id", "Name");
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(News news, IFormFile? ImageFile)
        {
            if (ModelState.IsValid || ImageFile != null) 
            {
                if (ImageFile != null)
                {
                    news.Image = await FileHelper.FileLoaderAsync(ImageFile, "img/News/");
                }

                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Authors = new SelectList(_context.Authors, "Id", "Name");
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");

            return View(news);
        }




        // GET: Admin/News/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", news.CategoryId);
            ViewBag.Authors = new SelectList(_context.Authors, "Id", "Name", news.AuthorId);
            return View(news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, News news, IFormFile? Image, bool cbResmiSil = false)
        {
            if (id != news.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingNews = await _context.News.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
                    if (existingNews == null)
                        return NotFound();

                    // Eski görselin yedeğini al
                    var oldImage = existingNews.Image;

                    // Eğer checkbox işaretliyse eski görseli sil
                    if (cbResmiSil && !string.IsNullOrEmpty(oldImage))
                    {
                        FileHelper.FileRemover("img/News/", oldImage);
                        news.Image = null;
                    }
                    else
                    {
                        news.Image = oldImage;
                    }

                    // Yeni görsel yüklendiyse eskisini silip yenisini yükle
                    if (Image != null)
                    {
                        if (!string.IsNullOrEmpty(oldImage))
                        {
                            FileHelper.FileRemover("img/News/", oldImage);
                        }

                        news.Image = await FileHelper.FileLoaderAsync(Image, "img/News/");
                    }

                    // Yayın tarihi değişmesin
                    news.PublishDate = existingNews.PublishDate;

                    _context.Entry(news).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // ViewBag'ler hata durumunda tekrar doldurulmalı
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name", news.CategoryId);
            ViewData["Authors"] = new SelectList(_context.Authors, "Id", "Name", news.AuthorId);
            return View(news);
        }





        // GET: Admin/News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                if (!string.IsNullOrWhiteSpace(news.Image))
                {
                    FileHelper.FileRemover(news.Image, "img/News/");
                }

                _context.News.Remove(news);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }



        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }
    }
}
