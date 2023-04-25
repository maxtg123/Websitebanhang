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
using Shop.Models;

namespace Shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class WarehousesController : Controller
    {
        private readonly ShopContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public WarehousesController(ShopContext context,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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

        public async Task<IActionResult> CheckUserLogin()
        {
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
            return View();
        }

        public async Task<IActionResult> Index()
        {
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

            var lstWarehouse = _context.Warehouses.Include(p => p.Products).OrderByDescending(p=> p.Id).ToList();
            if (TempData["Result"] != null)
            {
                ViewBag.ShowMessage = TempData["Result"];
            }

            foreach (var item in lstWarehouse)
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                item.PriceDisplay = item.Price.ToString("#,### ₫", cul.NumberFormat);

                item.Total = item.Price * item.Quantity;
                item.TotalDisplay = item.Total.ToString("#,### ₫", cul.NumberFormat);

                var pro = _context.Products.Where(p => p.Id == item.ProductId).FirstOrDefault();
                item.ProductName = pro.Name;
                item.ProductImage = pro.Image;
            }
            return View(lstWarehouse);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (warehouse == null)
            {
                return NotFound();
            }

            return View(warehouse);
        }

        public async Task<IActionResult> Create()
        {
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

            var shopContext = _context.Products.Include(p => p.Brand).Include(p => p.CPU).Include(p => p.ProductType).Include(p => p.Ram).Where(p => p.Status == true);
            if (TempData["Result"] != null)
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

        public async Task<IActionResult> CreateInStock(int id)
        {
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

            var product = _context.Products.Where(p => p.Id == id && p.Status == true).FirstOrDefault();
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
            product.PriceDisplay = product.Price.ToString("#,### ₫", cul.NumberFormat);
            return View(product);
        }

        public async Task<IActionResult> CreateNew()
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInStock([Bind("Id,ProductId,QuantityImport,PriceImport")] Warehouse warehouse)
        {
            var proId = warehouse.Id;
            DateTime dt = DateTime.Now;
            warehouse.ProductId = warehouse.Id;
            warehouse.Id = 0;
            warehouse.Quantity = warehouse.QuantityImport;
            warehouse.Price = warehouse.PriceImport;
            warehouse.IssuedDate = dt;
            _context.Add(warehouse);

            Product pro = await _context.Products.Where(p => p.Id == proId).FirstOrDefaultAsync();
            pro.Stock  = pro.Stock + warehouse.Quantity;
            _context.Update(pro);
            await _context.SaveChangesAsync();

            TempData["Result"] = MessageConst.CreateWarehouseSuccess;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNew([Bind("Id,SKU,Name,Description,Stock,Image,RamId,CPUId,BrandId,ProductTypeId,Status, ImageFile,Price,GalleryFiles,PricePay,QuantityImport,PriceImport")] Product product)
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
                    product.Stock = product.QuantityImport;
                    product.Price = product.PricePay;
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

                    if (product.Image == null)
                    {
                        product.Image = "no-image.png";
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                    }

                    //Nhiều hình ảnh
                    if (product.GalleryFiles != null)
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

                    Warehouse ware = new Warehouse();
                    ware.ProductId = product.Id;
                    ware.Quantity = product.QuantityImport;
                    ware.Price = product.PriceImport;
                    ware.IssuedDate = DateTime.Now;
                    _context.Add(ware);
                    _context.SaveChangesAsync();

                    TempData["Result"] = MessageConst.AddSuccess;
                    return RedirectToAction(nameof(Index));
                }
                return View(product);
            }
            else
            {
                ViewBag.UserLogin = null;
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }

    }
}
