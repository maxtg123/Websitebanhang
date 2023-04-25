using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Consts;
using Shop.Data;
using Shop.Helper;
using Shop.Models;

namespace Shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideshowsController : Controller
    {
        private readonly ShopContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SlideshowsController(ShopContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Kiểm tra User Login: Phúc 12/07/22
        public Account GetCurrentUserLogin()
        {
            Account userLogin = null;
            try
            {
                var userId = HttpContext.Session.GetInt32("userLoginId");
                if (userId != null)
                {
                    userLogin = _context.Accounts.FirstOrDefault(p => p.Id == userId && p.Status == true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return userLogin;
        }
        #endregion

        // GET: Admin/Slideshows
        public async Task<IActionResult> Index()
        {
            #region Kiểm tra User Login: Phúc 12/07/22
            Account userLogin = this.GetCurrentUserLogin();
            if (userLogin != null)
            {
                ViewBag.UserLogin = userLogin;
                if (!userLogin.IsAdmin)
                {
                    ViewBag.UserLogin = userLogin;
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }
            else
            {
                ViewBag.UserLogin = userLogin;
                return RedirectToAction("Index", "Login", new { area = "" });
            }
            #endregion
            return View(await _context.Slideshows.ToListAsync());
        }

        // GET: Admin/Slideshows/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slideshow = await _context.Slideshows
                .FirstOrDefaultAsync(m => m.Id == id);
            if (slideshow == null)
            {
                return NotFound();
            }

            return View(slideshow);
        }

        // GET: Admin/Slideshows/Create
        public IActionResult Create()
        {
            #region Kiểm tra User Login: Phúc 12/07/22
            Account userLogin = this.GetCurrentUserLogin();
            if (userLogin != null)
            {
                ViewBag.UserLogin = userLogin;
                if (!userLogin.IsAdmin)
                {
                    ViewBag.UserLogin = userLogin;
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }
            else
            {
                ViewBag.UserLogin = userLogin;
                return RedirectToAction("Index", "Login", new { area = "" });
            }
            #endregion
            return View();
        }

        // POST: Admin/Slideshows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Content,Image,ImageFile,Link")] Slideshow slideshow)
        {
            Account userLogin = this.GetCurrentUserLogin();
            if (userLogin != null)
            {
                if (!userLogin.IsAdmin)
                {
                    return Redirect("https://localhost:44347/Home");
                }

                if (ModelState.IsValid)
                {
                    _context.Add(slideshow);
                    await _context.SaveChangesAsync();

                    //  Upload HinhAnh
                    if (slideshow.ImageFile != null)
                    {
                        var fileName = slideshow.Id.ToString() + Path.GetExtension(slideshow.ImageFile.FileName);
                        var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "admin", "assets", "images", "slideshows");
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (FileStream fs = System.IO.File.Create(filePath))
                        {
                            slideshow.ImageFile.CopyTo(fs);
                            fs.Flush();
                        }
                        slideshow.Image = fileName;
                        _context.Update(slideshow);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        slideshow.Image = "no-image.png";
                        _context.Update(slideshow);
                        await _context.SaveChangesAsync();
                    }

                return RedirectToAction(nameof(Index));
                }
                return View(slideshow);

            }
            else
            {
                ViewBag.UserLogin = null;
                return Redirect("https://localhost:44347/Auth/Login");
            }

        }

        // GET: Admin/Slideshows/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            #region Kiểm tra User Login: Phúc 12/07/22
            Account userLogin = this.GetCurrentUserLogin();
            if (userLogin != null)
            {
                ViewBag.UserLogin = userLogin;
                if (!userLogin.IsAdmin)
                {
                    ViewBag.UserLogin = userLogin;
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }
            else
            {
                ViewBag.UserLogin = userLogin;
                return RedirectToAction("Index", "Login", new { area = "" });
            }
            #endregion

            if (id == null)
            {
                return NotFound();
            }

            var slideshow = await _context.Slideshows.FindAsync(id);
            if (slideshow == null)
            {
                return NotFound();
            }
            return View(slideshow);
        }

        // POST: Admin/Slideshows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Content,Image,ImageFile,Link")] Slideshow slideshow)
        {
            if (id != slideshow.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(slideshow);
                    await _context.SaveChangesAsync();

                    //  Upload HinhAnh
                    if (slideshow.ImageFile != null)
                    {
                        //  Xóa ảnh hiển thị
                        var fileToDelete = Path.Combine(_webHostEnvironment.WebRootPath, "admin", "assets", "images", "slideshows", slideshow.Image);
                        FileInfo fi = new FileInfo(fileToDelete);
                        fi.Delete();

                        //Hình ảnh hiển thị
                        var fileName = slideshow.Id.ToString() + Path.GetExtension(slideshow.ImageFile.FileName);
                        var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "admin", "assets", "images", "slideshows");
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (FileStream fs = System.IO.File.Create(filePath))
                        {
                            slideshow.ImageFile.CopyTo(fs);
                            fs.Flush();
                        }
                        slideshow.Image = fileName;
                        _context.Update(slideshow);
                        await _context.SaveChangesAsync();
                    }

                    if (slideshow.Image == null)
                    {
                        slideshow.Image = "no-image.png";
                        _context.Update(slideshow);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SlideshowExists(slideshow.Id))
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
            return View(slideshow);
        }

        // GET: Admin/Slideshows/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            #region Kiểm tra User Login: Phúc 12/07/22
            Account userLogin = this.GetCurrentUserLogin();
            if (userLogin != null)
            {
                ViewBag.UserLogin = userLogin;
                if (!userLogin.IsAdmin)
                {
                    ViewBag.UserLogin = userLogin;
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }
            else
            {
                ViewBag.UserLogin = userLogin;
                return RedirectToAction("Index", "Login", new { area = "" });
            }
            #endregion

            if (id == null)
            {
                return NotFound();
            }

            var slideshow = await _context.Slideshows
                .FirstOrDefaultAsync(m => m.Id == id);
            _context.Remove(slideshow);
            await _context.SaveChangesAsync();
            if (slideshow == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool SlideshowExists(int id)
        {
            return _context.Slideshows.Any(e => e.Id == id);
        }
    }
}
