using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class ProductsController : Controller
    {
        private readonly ShopContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ShopContext context, IWebHostEnvironment iWebHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = iWebHostEnvironment;
        }

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

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            //Kiểm tra xem đã login? và kiểm tra user thường hay admin
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
            var shopContext = _context.Products.Include(p => p.Brand).Include(p => p.CPU).Include(p => p.ProductType).Include(p => p.Ram).Where(p=> p.Status == true);
            if(TempData["Result"] != null)
            {
                ViewBag.ShowMessage = TempData["Result"];
            }

            foreach (var item in shopContext)
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                item.PriceDisplay = item.Price.ToString("#,### ₫", cul.NumberFormat);
            }

            return View(await shopContext.ToListAsync());
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //Kiểm tra xem đã login? và kiểm tra user thường hay admin
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

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
            product.PriceDisplay = product.Price.ToString("#,### ₫", cul.NumberFormat);

            product.ProductImages = await _context.ProductImages.Where(p => p.ProductId == product.Id).ToListAsync();
            if (product == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");
            ViewData["CPUId"] = new SelectList(_context.CPUs, "Id", "Name");
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name");
            ViewData["RamId"] = new SelectList(_context.Rams, "Id", "Amount");
            ViewBag.ProcessHis = await _context.ProcessHistories.Include(p => p.Account).Where(p => p.HandleWhatId == id).ToListAsync();
            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            //Kiểm tra xem đã login? và kiểm tra user thường hay admin
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

            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");
            ViewData["CPUId"] = new SelectList(_context.CPUs, "Id", "Name");
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name");
            ViewData["RamId"] = new SelectList(_context.Rams, "Id", "Amount");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SKU,Name,Description,Stock,Image,RamId,CPUId,BrandId,ProductTypeId,Status, ImageFile,Price,GalleryFiles")] Product product)
        {
            Account userLogin = this.GetCurrentUserLogin();
            if (userLogin != null)
            {
                if (!userLogin.IsAdmin)
                {
                    return RedirectToAction("Index", "Home", new { area = "" });
                }

                if (ModelState.IsValid)
                {
                    product.Status = true;
                    _context.Add(product);
                    await _context.SaveChangesAsync();

                    //  Upload HinhAnh
                    if (product.ImageFile != null)
                    {
                        //Hình ảnh hiển thị
                        var fileName = product.Id.ToString() + Path.GetExtension(product.ImageFile.FileName);
                        var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "admin", "assets", "images", "products");
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (FileStream fs = System.IO.File.Create(filePath))
                        {
                            product.ImageFile.CopyTo(fs);
                            fs.Flush();
                        }
                        product.Image = fileName;
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                    }
                    
                    if(product.Image == null)
                    {
                        product.Image = "no-image.png";
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                    }

                    //Nhiều hình ảnh
                    if(product.GalleryFiles != null)
                    {
                        int i = 1;
                        foreach (var file in product.GalleryFiles)
                        {
                            var fileName2 = product.Id.ToString() + "-" + i + Path.GetExtension(file.FileName);
                            var uploadPath2 = Path.Combine(_webHostEnvironment.WebRootPath, "admin", "assets", "images", "productDetails");
                            var filePath2 = Path.Combine(uploadPath2, fileName2);
                            using (FileStream fs = System.IO.File.Create(filePath2))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }

                            ProductImage proImage = new ProductImage();
                            proImage.ProductId = product.Id;
                            proImage.Image = fileName2;

                            _context.Update(proImage);
                            await _context.SaveChangesAsync();
                            i++;
                        }
                    }


                    ProcessHistory process = new ProcessHistory();
                    process.Content = ProcessContst.Add;
                    process.HandleWhat = ProcessWhat.Product;
                    process.HandleWhatId = product.Id;
                    process.ProcessDT = DateTime.Now;
                    process.AccountId = userLogin.Id;
                    _context.Add(process);
                    await _context.SaveChangesAsync();

                    TempData["Result"] = MessageConst.AddSuccess;
                    return RedirectToAction(nameof(Index));
                }
                ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");
                ViewData["CPUId"] = new SelectList(_context.CPUs, "Id", "Name");
                ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name");
                ViewData["RamId"] = new SelectList(_context.Rams, "Id", "Amount");
                return View();
            }
            else
            {
                ViewBag.UserLogin = null;
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //Kiểm tra xem đã login? và kiểm tra user thường hay admin
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

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            product.ProductImages = await _context.ProductImages.Where(p=> p.ProductId == product.Id).ToListAsync();
            if (product == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");
            ViewData["CPUId"] = new SelectList(_context.CPUs, "Id", "Name");
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name");
            ViewData["RamId"] = new SelectList(_context.Rams, "Id", "Amount");
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SKU,Name,Description,Stock,Image,RamId,CPUId,BrandId,ProductTypeId,Status,Price,ImageFile,GalleryFiles")] Product product)
        {
            //Kiểm tra xem đã login? và kiểm tra user thường hay admin
            Account userLogin = this.GetCurrentUserLogin();
            if (userLogin != null)
            {
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

            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.Status = true;
                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    //  Upload HinhAnh
                    if (product.ImageFile != null)
                    {
                        //  Xóa ảnh hiển thị
                        var fileToDelete = Path.Combine(_webHostEnvironment.WebRootPath, "admin", "assets", "images", "products", product.Image);
                        FileInfo fi = new FileInfo(fileToDelete);
                        fi.Delete();

                        //Hình ảnh hiển thị
                        var fileName = product.Id.ToString() + Path.GetExtension(product.ImageFile.FileName);
                        var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "admin", "assets", "images", "products");
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (FileStream fs = System.IO.File.Create(filePath))
                        {
                            product.ImageFile.CopyTo(fs);
                            fs.Flush();
                        }
                        product.Image = fileName;
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                    }

                    if(product.Image == null)
                    {
                        product.Image = "no-image.png";
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                    }

                    //Nhiều hình ảnh
                    if (product.GalleryFiles != null)
                    {
                        //  Xóa ảnh chi tiết
                        var lstImgDetail = _context.ProductImages.Where(p => p.ProductId == product.Id).ToList();
                        foreach (var file in lstImgDetail)
                        {
                            var fileToDeleteDetail = Path.Combine(_webHostEnvironment.WebRootPath, "admin", "assets", "images", "productDetails", file.Image);
                            FileInfo fiDetail = new FileInfo(fileToDeleteDetail);
                            fiDetail.Delete();

                            _context.ProductImages.Remove(file);
                            await _context.SaveChangesAsync();
                        }
                        

                        int i = 1;
                        foreach (var file in product.GalleryFiles)
                        {
                            var fileName2 = product.Id.ToString() + "-" + i + Path.GetExtension(file.FileName);
                            var uploadPath2 = Path.Combine(_webHostEnvironment.WebRootPath, "admin", "assets", "images", "productDetails");
                            var filePath2 = Path.Combine(uploadPath2, fileName2);
                            using (FileStream fs = System.IO.File.Create(filePath2))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }

                            ProductImage proImage = new ProductImage();
                            proImage.ProductId = product.Id;
                            proImage.Image = fileName2;

                            _context.Update(proImage);
                            await _context.SaveChangesAsync();
                            i++;
                        }
                    }

                    ProcessHistory process = new ProcessHistory();
                    process.Content = ProcessContst.Edit;
                    process.HandleWhat = ProcessWhat.Product;
                    process.HandleWhatId = product.Id;
                    process.ProcessDT = DateTime.Now;
                    process.AccountId = userLogin.Id;
                    _context.Add(process);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["Result"] = MessageConst.UpdateSuccess;
                return RedirectToAction(nameof(Index));
            }
            //ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            //ViewData["CPUId"] = new SelectList(_context.CPUs, "Id", "Name", product.CPUId);
            //ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name", product.ProductTypeId);
            //ViewData["RamId"] = new SelectList(_context.Rams, "Id", "Amount", product.RamId);
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            //Kiểm tra xem đã login? và kiểm tra user thường hay admin
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

            var product = await _context.Products.FindAsync(id);
            product.Status = false;
            _context.Update(product);
            //_context.Products.Remove(product);
            await _context.SaveChangesAsync();

            ProcessHistory process = new ProcessHistory();
            process.Content = ProcessContst.Delete;
            process.HandleWhat = ProcessWhat.Product;
            process.HandleWhatId = product.Id;
            process.ProcessDT = DateTime.Now;
            process.AccountId = userLogin.Id;
            _context.Add(process);
            await _context.SaveChangesAsync();

            TempData["Result"] = MessageConst.DeleteSuccess;
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
