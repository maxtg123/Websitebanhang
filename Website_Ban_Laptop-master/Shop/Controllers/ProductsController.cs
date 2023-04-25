using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ShopContext _context;

        public ProductsController(ShopContext context)
        {
            _context = context;
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

        // GET: Products
        public async Task<IActionResult> Index(int? id)
        {
            Account userLogin = this.GetCurrentUserLogin();
            if (userLogin != null)
            {
                ViewBag.UserLogin = userLogin;
                ViewBag.TotalCart = _context.Carts.Where(p => p.AccountId == userLogin.Id).Count();
            }

            var lstBrands = await _context.Brands.Where(p => p.Status == true).ToListAsync();
            foreach (var item in lstBrands)
            {
                item.ProductCount = _context.Products.Count(p => p.BrandId == item.Id && p.Status == true);
            }
            ViewBag.lstBrands = lstBrands;

            var shopContext = _context.Products.Where(p => p.Status == true).Include(p => p.Brand).Include(p => p.ProductType);
            foreach (var item in shopContext)
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                string priceFormat = item.Price.ToString("#,###", cul.NumberFormat);
                item.PriceDisplay = priceFormat;
            }

            if (id != null && id != 0)
            {
                var shopContext1 = await _context.Products.
                                    Where(p => p.Status == true).
                                    Include(p => p.Brand).
                                    Include(p => p.ProductType).
                                    Where(p => p.Status == true && p.Brand.Id == id).ToListAsync();
                if (shopContext1.Count() > 0)
                {
                    foreach (var item in shopContext1)
                    {
                        CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                        string priceFormat = item.Price.ToString("#,###", cul.NumberFormat);
                        item.PriceDisplay = priceFormat;
                    }
                }

                return View(shopContext1);
            }

            return View(await shopContext.ToListAsync());
        }

        public async Task<IActionResult> Product_Filter(string? searchString, int? Id)
        {
            Account userLogin = this.GetCurrentUserLogin();
            if (userLogin != null)
            {
                ViewBag.UserLogin = userLogin;
            }

            ViewBag.lstBrands = await _context.Brands.Where(p => p.Status == true).ToListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                var shopContext = await _context.Products.
                                    Where(p => p.Status == true).
                                    Include(p => p.Brand).
                                    Include(p => p.ProductType).
                                    Where(p => p.Status == true && p.Name.ToUpper().Contains(searchString.ToUpper()) 
                                    || p.Brand.Name.ToUpper().Contains(searchString.ToUpper())).ToListAsync();

                
                foreach (var item in shopContext)
                {
                    CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                    string priceFormat = item.Price.ToString("#,###", cul.NumberFormat);
                    item.PriceDisplay = priceFormat;
                }

                ViewBag.TitleFilter = "Hiển thị " + shopContext.Count() + " kết quả cho '" + searchString + "'";

                return View(shopContext);
            }

            if (Id != null && Id != 0)
            {
                var shopContext1 = await _context.Products.
                                    Where(p => p.Status == true).
                                    Include(p => p.Brand).
                                    Include(p => p.ProductType).
                                    Where(p=> p.Status == true && p.Brand.Id == Id).ToListAsync();
                if(shopContext1.Count() > 0)
                {
                    foreach (var item in shopContext1)
                    {
                        CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                        string priceFormat = item.Price.ToString("#,###", cul.NumberFormat);
                        item.PriceDisplay = priceFormat;
                    }
                    ViewBag.TitleFilter = shopContext1[0].Brand.Name;
                }
                else
                {
                    ViewBag.TitleFilter = "Không tìm thấy kết quả";
                }

                return View(shopContext1);
            }

            return View();
        }

        public async Task<IActionResult> Product_Detail(int? id)
        {
            Account userLogin = this.GetCurrentUserLogin();
            if (userLogin != null)
            {
                ViewBag.UserLogin = userLogin;
                ViewBag.TotalCart = _context.Carts.Where(p => p.AccountId == userLogin.Id).Count();
            }
            if (TempData["ResultUI"] != null)
            {
                ViewBag.ResultUI = TempData["ResultUI"];
            }

            var pro = _context.Products.FirstOrDefault(p => p.Status == true && p.Id == id);
            //Update View
            pro.View++;
            _context.Update(pro);
            await _context.SaveChangesAsync();


            ViewBag.lstBrands = await _context.Brands.Where(p => p.Status == true).ToListAsync();
            ViewBag.lstCPUs = await _context.CPUs.Where(p => p.Status == true).ToListAsync();
            ViewBag.lstRams = await _context.Rams.Where(p => p.Status == true).ToListAsync();
            ViewBag.lstProductTypes = await _context.ProductTypes.Where(p => p.Status == true).ToListAsync();
            ViewBag.lstProductsGaming = await _context.Products.Include(p => p.Brand).Where(p => p.Status == true && p.ProductType.Name.Contains("GAMING")).ToListAsync();
            ViewBag.lstProductsVP = await _context.Products.Include(p => p.Brand).Where(p => p.Status == true && p.ProductType.Name.Contains("HỌC TẬP, VĂN PHÒNG")).ToListAsync();
            //ViewBag.lstBrands = await _context.Brands.Where(p => p.Status == true).ToListAsync();
            var ProductDetail = await _context.Products
                                    .Include(p => p.Brand)
                                    .Include(p => p.ProductImages)
                                    .Include(p => p.ProductType)
                                    .Where(p=> p.Id == id).ToListAsync();
            ViewBag.lstImage = await _context.ProductImages.Where(p => p.ProductId == id).ToListAsync();

            if (ProductDetail == null)
            {
                return NotFound();
            }
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
            string priceFormat = ProductDetail[0].Price.ToString("#,###", cul.NumberFormat);
            ProductDetail[0].PriceDisplay = priceFormat;

            //Sản phẩm liên quan
            var relateToProduct = await _context.Products.Include(p => p.Brand).Where(p => p.BrandId == ProductDetail[0].BrandId && p.Status == true).ToListAsync();
            foreach (var item in relateToProduct)
            {
                CultureInfo cul1 = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                string priceFormat1 = item.Price.ToString("#,###", cul.NumberFormat);
                item.PriceDisplay = priceFormat1;
            }
            ViewBag.relateToProduct = relateToProduct;

            return View(ProductDetail);
        }

    }
}
