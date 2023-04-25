using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
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
    public class InvoicesController : Controller
    {
        private readonly ShopContext _context;

        public InvoicesController(ShopContext context)
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

        // GET: Admin/Invoices
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
            var shopContext = _context.Invoices.Include(i => i.Account).Include(i => i.OrderStatus);
            if (TempData["Result"] != null)
            {
                ViewBag.ShowMessage = TempData["Result"];
            }

            foreach (var item in shopContext)
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                item.TotalDisplay = item.Total.ToString("#,### ₫", cul.NumberFormat);
            }

            return View(await shopContext.ToListAsync());
        }

        // GET: Admin/Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
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
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Account)
                .Include(i => i.OrderStatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Admin/Invoices/Create
        public IActionResult Create()
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username");
            ViewData["OrderStatusId"] = new SelectList(_context.OrderStatuses, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,AccountId,IssuedDate,ShippingName,ShippingAddress,ShippingPhone,ShipperName,ShipperPhone,Notes,Total,OrderStatusId")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", invoice.AccountId);
            ViewData["OrderStatusId"] = new SelectList(_context.OrderStatuses, "Id", "Id", invoice.OrderStatusId);
            return View(invoice);
        }

        // GET: Admin/Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", invoice.AccountId);
            ViewData["OrderStatusId"] = new SelectList(_context.OrderStatuses, "Id", "Id", invoice.OrderStatusId);
            return View(invoice);
        }

        // POST: Admin/Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,AccountId,IssuedDate,ShippingName,ShippingAddress,ShippingPhone,ShipperName,ShipperPhone,Notes,Total,OrderStatusId")] Invoice invoice)
        {
            if (id != invoice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", invoice.AccountId);
            ViewData["OrderStatusId"] = new SelectList(_context.OrderStatuses, "Id", "Id", invoice.OrderStatusId);
            return View(invoice);
        }

        public async Task<IActionResult> UpdateOrder(int id, int orderId)
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
            var invoice = await _context.Invoices.Where(p => p.Id == id).FirstOrDefaultAsync();
            if (orderId != 6)
            {
                invoice.OrderStatusId = orderId + 1;
            }
            else
            {
                invoice.OrderStatusId = orderId;
            }
            _context.Update(invoice);
            _context.SaveChanges();
            TempData["Result"] = MessageConst.UpdateOrdersuccess;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> InvoiceDetail(int id)
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
            var invoiceDetails = await _context.InvoiceDetails.Where(p => p.InvoiceId == id).Include(p => p.Product).Include(p => p.Invoice).ToListAsync();
            foreach (var item in invoiceDetails)
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                item.UnitPriceDisplay = item.UnitPrice.ToString("#,### ₫", cul.NumberFormat);
                ViewBag.TotalInvoice = item.Invoice.TotalDisplay = item.Invoice.Total.ToString("#,### ₫", cul.NumberFormat);
                item.IntoMoney = (item.UnitPrice * item.Quantity).ToString("#,### ₫", cul.NumberFormat);
            }

            return View(invoiceDetails);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateShipper(string ShipperName, string ShipperPhone, int id)
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
            var invoice = await _context.Invoices.Where(p => p.Id == id).FirstOrDefaultAsync();
            invoice.OrderStatusId += 1;
            invoice.ShipperName = ShipperName;
            invoice.ShipperPhone = ShipperPhone;
            _context.Update(invoice);
            _context.SaveChanges();
            TempData["Result"] = MessageConst.UpdateOrdersuccess;
            return RedirectToAction(nameof(Index));
        }

        class StaticticalOb {
            public int Id;
            public string Name;
        }

        public IActionResult StaticticalCreate()
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
            List<StaticticalOb> lstStatictical = new List<StaticticalOb>()
            {
                new StaticticalOb{Id = 1, Name = "Thống kê doanh thu từ ngày - ngày"},
                new StaticticalOb{Id = 2, Name = "Thống kê doanh thu theo tháng"},
                new StaticticalOb{Id = 3, Name = "Thống kê doanh thu theo năm"}
            };

            ViewData["lstStatictical"] = lstStatictical;

            DateTime dt = DateTime.Now;
            var dtSplit = dt.ToString().Split('/');
            string startD = "1/"+ dtSplit[0]+ '/' + dtSplit[2].Substring(0,4);
            string endD = dtSplit[1] + '/' + dtSplit[0] + '/' + dtSplit[2].Substring(0, 4);
            this.Statictical(startD, endD, null, 1);
            return View();
        }

        public async Task<IActionResult> Statictical(string startD, string endD, string all, int type)
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
            //Thống kê doanh thu từ ngày -> ngày
            if (startD != null && endD == null && type != 1 || startD != null && endD != null)
            {
                var startDSplit = startD.Split('/');
                var endDSplit = endD.Split('/');

                DateTime startDate = new DateTime(Convert.ToInt32(startDSplit[2]), Convert.ToInt32(startDSplit[1]), Convert.ToInt32(startDSplit[0]));
                DateTime endDate = new DateTime(Convert.ToInt32(endDSplit[2]), Convert.ToInt32(endDSplit[1]), Convert.ToInt32(endDSplit[0]));

                if (type == 1)
                {
                    DateTime end = endDate.AddDays(1);
                    var lstHD = _context.Invoices.
                        Include(p => p.Account).
                        Include(p => p.OrderStatus).
                        Where(p => p.IssuedDate >= startDate && p.IssuedDate <= end && p.OrderStatusId == 5).
                        OrderByDescending(p => p.IssuedDate).ToList();

                    CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                    ViewBag.TotalFromDocToDoc = lstHD.Sum(p => p.Total).ToString("#,### ₫", cul.NumberFormat);

                    foreach (var item in lstHD)
                    {
                        item.TotalDisplay = item.Total.ToString("#,### ₫", cul.NumberFormat);
                    }

                    ViewBag.TitleStatictical = " từ " + startD + " đến " + endD;
                    return View("StaticticalCreate", lstHD);
                }
                else if (type == 2)
                {
                    var lstHD = _context.Invoices.
                        Include(p => p.Account).
                        Include(p => p.OrderStatus).
                        Where(p => p.IssuedDate.Month == Convert.ToInt32(startDSplit[1]) && p.OrderStatusId == 5).
                        OrderByDescending(p => p.IssuedDate).ToList();

                    CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                    ViewBag.TotalFromDocToDoc = lstHD.Sum(p => p.Total).ToString("#,### ₫", cul.NumberFormat);

                    foreach (var item in lstHD)
                    {
                        item.TotalDisplay = item.Total.ToString("#,### ₫", cul.NumberFormat);
                    }

                    ViewBag.TitleStatictical = " trong tháng " + startDSplit[1];
                    return View("StaticticalCreate", lstHD);
                }
                else if (type == 3)
                {
                    var lstHD = _context.Invoices.
                        Include(p => p.Account).
                        Include(p => p.OrderStatus).
                        Where(p => p.IssuedDate.Year == Convert.ToInt32(startDSplit[2]) && p.OrderStatusId == 5).
                        OrderByDescending(p => p.IssuedDate).ToList();

                    CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                    ViewBag.TotalFromDocToDoc = lstHD.Sum(p => p.Total).ToString("#,### ₫", cul.NumberFormat);

                    foreach (var item in lstHD)
                    {
                        item.TotalDisplay = item.Total.ToString("#,### ₫", cul.NumberFormat);
                    }

                    ViewBag.TitleStatictical = " trong năm " + startDSplit[2];
                    return View("StaticticalCreate", lstHD);
                }
            }

            //Thống kê doanh thu theo tháng

            //Thống kê doanh thu theo năm

            //Thống kê sản phẩm bán chạy

            return View("StaticticalCreate");
        }
    }
}
