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
    public class SlidersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SlidersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Sliders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Slider.ToListAsync());
        }

        // GET: Admin/Sliders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Slider
                .FirstOrDefaultAsync(m => m.Id == id);
            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        // GET: Admin/Sliders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Sliders/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider sliders, IFormFile? ImageFile)
        {
            if (ModelState.IsValid || ImageFile != null)
            {
                if (ImageFile != null)
                {
                    sliders.Image = await FileHelper.FileLoaderAsync(ImageFile, "img/Sliders/");
                }

                _context.Add(sliders);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }



            return View(sliders);
        }

        // GET: Admin/Sliders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Slider.FindAsync(id);
            if (slider == null)
            {
                return NotFound();
            }
            return View(slider);
        }

        // POST: Admin/Sliders/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Slider sliders, IFormFile? Image, bool cbResmiSil = false)
        {
            if (id != sliders.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSlider = await _context.Slider.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                    if (existingSlider == null)
                        return NotFound();

                    var oldImage = existingSlider.Image;

                    // Resim silme checkbox'ı işaretli ise
                    if (cbResmiSil && !string.IsNullOrEmpty(oldImage))
                    {
                        FileHelper.FileRemover(oldImage, "img/Sliders/");
                        sliders.Image = null;
                    }
                    else
                    {
                        sliders.Image = oldImage;
                    }

                    // Yeni resim yüklendiyse
                    if (Image != null)
                    {
                        if (!string.IsNullOrEmpty(oldImage))
                        {
                            FileHelper.FileRemover(oldImage, "img/Sliders/");
                        }

                        sliders.Image = await FileHelper.FileLoaderAsync(Image, "img/Sliders/");
                    }

                    _context.Update(sliders); // Güncelleme işlemi
                    await _context.SaveChangesAsync(); // ❗️Eksikti: Veritabanına işle
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SliderExists(sliders.Id))
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

            return View(sliders);
        }


        // GET: Admin/Sliders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Slider
                .FirstOrDefaultAsync(m => m.Id == id);
            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        // POST: Admin/Sliders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sliders = await _context.Slider.FindAsync(id);
            if (sliders != null)
            {
                if (!string.IsNullOrWhiteSpace(sliders.Image))
                {
                    FileHelper.FileRemover(sliders.Image, "img/Sliders/");
                }

                _context.Slider.Remove(sliders);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SliderExists(int id)
        {
            return _context.Slider.Any(e => e.Id == id);
        }
    }
}
