using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShopContext _context;

        public HomeController(ShopContext context) 
        {
            _context = context;
        }

        public class ProBestSeller
        {
            public int Id { get; set; }
            public string ProductName { get; set; }
            public string ProductImage { get; set; }
            public int TotalQuantity { get; set; }
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
        public async Task<IActionResult> Index()
        {
            Account userLogin = this.GetCurrentUserLogin();
            if (userLogin != null)
            {
                 ViewBag.UserLogin = userLogin;
                int totalCart = _context.Carts.Where(p => p.AccountId == userLogin.Id).Count();
                ViewBag.TotalCart = totalCart;
            }


            ViewBag.lstBrands = await _context.Brands.Where(p => p.Status == true).ToListAsync();
            ViewBag.lstSlideShows = await _context.Slideshows.ToListAsync();
            var lstProductVP = await _context.Products.Include(p => p.Brand).Where(p => p.Status == true && p.ProductType.Name.Contains("HỌC TẬP, VĂN PHÒNG")).ToListAsync();
            var lstProductGameming = await _context.Products.Include(p => p.Brand).Where(p => p.Status == true && p.ProductType.Name.Contains("GAMING")).ToListAsync();
            if(lstProductGameming.Count > 0)
            {
                foreach (var item in lstProductGameming)
                {
                    CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                    string priceFormat = item.Price.ToString("#,###", cul.NumberFormat);
                    item.PriceDisplay = priceFormat;
                }
            }

            if (lstProductVP.Count > 0)
            {
                foreach (var item in lstProductVP)
                {
                    CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                    string priceFormat = item.Price.ToString("#,###", cul.NumberFormat);
                    item.PriceDisplay = priceFormat;
                }
            }
            #region Danh sách sản phẩm bán chạy nhất
            var proBestSeller = _context.InvoiceDetails.
                                Include(p => p.Product).
                                Include(p => p.Invoice).
                                Where(p => p.InvoiceId == p.Invoice.Id && p.Invoice.OrderStatusId == 5).ToList();
            if (proBestSeller != null)
            {
                var proBestSellers = proBestSeller.
                                    GroupBy(p => p.ProductId).
                                    SelectMany(pro => pro.Select(proNew => new ProBestSeller
                                    {
                                        Id = proNew.Product.Id,
                                        TotalQuantity = pro.Sum(p => p.Quantity)
                                    })).Distinct().OrderByDescending(p => p.TotalQuantity).ToList();

                List<ProBestSeller> lstProDistinct = new List<ProBestSeller>();
                List<Product> lstPro = new List<Product>();

                foreach (var item in proBestSellers)
                {
                    var checkPro = lstProDistinct.Exists(p => p.Id == item.Id);
                    if (!checkPro)
                    {
                        var foundPro = _context.Products.Include(p => p.Brand).Where(p => p.Id == item.Id).FirstOrDefault();
                        CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                        string priceFormat = foundPro.Price.ToString("#,###", cul.NumberFormat);
                        foundPro.PriceDisplay = priceFormat;
                        lstPro.Add(foundPro);
                        lstProDistinct.Add(item);
                    }
                }

                ViewBag.proBestSeller = lstPro;
            }
            #endregion

            #region Danh sách sản phẩm xem nhiều nhất
            var proMostView = await _context.Products
                                    .Include(p => p.Brand)
                                    .Include(p => p.ProductType)
                                    .Where(p => p.Status == true).OrderByDescending(p=> p.View).ToListAsync();

            foreach (var item in proMostView)
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                string priceFormat = item.Price.ToString("#,###", cul.NumberFormat);
                item.PriceDisplay = priceFormat;
            }

            ViewBag.proMostView = proMostView;
            #endregion

            ViewBag.lstProductsGaming = lstProductGameming;
            ViewBag.lstProductsVP = lstProductVP;
            return View();
        }
    }
}
