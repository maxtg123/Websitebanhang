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

namespace Shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ShopContext _context;

        public DashboardController(ShopContext context)
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

        public class UserRick
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string UserImage { get; set; }
            public string Phone { get; set; }
            public int Total { get; set; }
            public string TotalDisplay { get; set; }
        }

        public IActionResult Index()
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
            var productCount = _context.Products.Where(p => p.Status == true).Count();
            var productNotifi = _context.Products.Where(p => p.Status == true && p.Stock < 10).Count();
            var invoiceCount = _context.Invoices.Where(p=> p.OrderStatusId == 1).Count();
            var accountCount = _context.Accounts.Where(p => p.Status == true).Count();

            //if(productCount != 0 && productCount != null)
            //{
            //    ViewBag.productCount = productCount;
            //}
            //else
            //{
            //    ViewBag.productCount = 0;
            //}

            if (productNotifi != 0 && productNotifi != null)
            {
                ViewBag.productNotifi = productNotifi;
            }
            else
            {
                ViewBag.productNotifi = 0;
            }

            if (invoiceCount != 0 && invoiceCount != null)
            {
                ViewBag.invoiceCount = invoiceCount;
            }
            else
            {
                ViewBag.invoiceCount = 0;
            }

            if (accountCount != 0 && accountCount != null)
            {
                ViewBag.accountCount = accountCount;
            }
            else
            {
                ViewBag.accountCount = 0;
            }

            //Danh sách sản phẩm bán chạy nhất
            DateTime dt = DateTime.Now;
            var month = dt.Month;
            var year = dt.Year;
            ViewBag.TitleProductNotifi = "Top 5 sản phẩm bán chạy trong " + month + '/' + year;
            ViewBag.TitleUserRick = "Top 5 khách hàng có tổng tiền hoá đơn nhiều nhất trong " + month + '/' + year;

            var proBestSeller = _context.InvoiceDetails.
                                Include(p => p.Product).
                                Include(p=> p.Invoice).
                                Where(p=> p.InvoiceId == p.Invoice.Id && p.Invoice.OrderStatusId == 5 && p.Invoice.IssuedDate.Month == month && p.Invoice.IssuedDate.Year == year).ToList();

            var proBestSellers = proBestSeller.
                                GroupBy(p => p.ProductId).
                                SelectMany(pro => pro.Select(proNew => new ProBestSeller
                                {
                                    Id = proNew.Product.Id,
                                    ProductName = proNew.Product.Name,
                                    ProductImage = proNew.Product.Image,
                                    TotalQuantity = pro.Sum(c=> c.Quantity),
                                })).Distinct().OrderByDescending(p=> p.TotalQuantity).ToList();
            List<ProBestSeller> lstPro = new List<ProBestSeller>();
            foreach (var item in proBestSellers)
            {
                var found =  lstPro.Where(p => p.Id == item.Id).FirstOrDefault();
                if(found == null)
                {
                    lstPro.Add(item);
                }
            }
            ViewBag.proBestSeller = lstPro.Take(5);

            //Danh sách khách hàng mua hàng với tổng tiền nhiều nhất
            var userRick = _context.Invoices.
                            Include(p => p.Account).
                            Where(p => p.OrderStatusId == 5).ToList();

            var userRicks = userRick.
                            GroupBy(p => p.AccountId).
                            SelectMany(inv => inv.Select(u => new UserRick
                            {
                                Id = u.Account.Id,
                                FullName = u.Account.FullName,
                                UserImage = u.Account.Avatar,
                                Phone = u.Account.Phone,
                                Total = inv.Sum(p=> p.Total)
                            })).Distinct().ToList();
            
            List<UserRick> lstUserRicks = new List<UserRick>();
            foreach (var item in userRicks)
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                item.TotalDisplay = item.Total.ToString("#,### ₫", cul.NumberFormat);

                var found = lstUserRicks.Where(p => p.Id == item.Id).FirstOrDefault();
                if (found == null)
                {
                    lstUserRicks.Add(item);
                }
            }
            ViewBag.UserRick = lstUserRicks.OrderByDescending(p=> p.Total);
            return View();
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
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        public async Task<IActionResult> ProductNotification()
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
            var lstProNotifi = await _context.Products
                                            .Include(p=> p.Brand)
                                            .Where(p => p.Status == true && p.Stock < 10).OrderByDescending(p=> p.Stock).ToListAsync();
            foreach (var item in lstProNotifi)
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                item.PriceDisplay = item.Price.ToString("#,### ₫", cul.NumberFormat);
            }

            return View(lstProNotifi);
        }

        public async Task<IActionResult> InvoiceWaitingConfirm(int? orderStatus)
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
            var shopContext = _context.Invoices
                                .Include(i => i.Account)
                                .Include(i => i.OrderStatus)
                                .Where(p=> p.OrderStatusId == orderStatus);

            foreach (var item in shopContext)
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                item.TotalDisplay = item.Total.ToString("#,###", cul.NumberFormat);
            }

            return View(await shopContext.ToListAsync());
        }

        public async Task<IActionResult> ProductSellerDetail(int? id)
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



            var shopContext = await _context.InvoiceDetails
                                            .Include(p => p.Invoice)
                                            .Include(p=> p.Product)
                                            .Where(p => p.ProductId == id && p.Invoice.OrderStatusId == 5)
                                            .ToListAsync();

            ViewBag.TitleProductSellserDetail = " " + shopContext[0].Product.Name;

            List<Invoice> lstInvoice = new List<Invoice>();
            foreach (var item in shopContext)
            {
                var invoice = lstInvoice.Count(p => p.Id == item.InvoiceId);

                if(invoice == 0)
                {
                    var invoiceAdd = _context.Invoices.Include(p => p.OrderStatus).Include(p => p.Account).FirstOrDefault(p => p.Id == item.InvoiceId);
                    CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                    invoiceAdd.TotalDisplay = invoiceAdd.Total.ToString("#,###", cul.NumberFormat);
                    lstInvoice.Add(invoiceAdd);
                }
            }
            ViewBag.lstInvoice = lstInvoice;
            return View();
        }

        public async Task<IActionResult> UserRickDetail(int? id)
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



            var shopContext = await _context.Invoices.Include(p => p.Account).Include(p => p.OrderStatus).Where(p => p.OrderStatusId == 5 && p.AccountId == id).ToListAsync();

            ViewBag.TitleProductSellserDetail = " " + shopContext[0].Account.FullName;

            foreach (var item in shopContext)
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                item.TotalDisplay = item.Total.ToString("#,###", cul.NumberFormat);
            }
            return View(shopContext);
        }

    }
}
