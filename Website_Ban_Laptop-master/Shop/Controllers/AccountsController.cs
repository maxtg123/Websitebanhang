using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Consts;
using Shop.Data;
using Shop.Models;
using static System.Formats.Asn1.AsnWriter;

namespace Shop.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ShopContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountsController(ShopContext context, IWebHostEnvironment iWebHostEnvironment)
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
        // GET: Accounts
        public async Task<IActionResult> Index(int id)
        {
            Account userLogin = this.GetCurrentUserLogin();
            if (userLogin != null)
            {
                ViewBag.UserLogin = userLogin;
                int totalCart = _context.Carts.Where(p => p.AccountId == userLogin.Id).Count();
                ViewBag.TotalCart = totalCart;
            }
            else
            {
                ViewBag.UserLogin = userLogin;
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var account = await _context.Accounts.FindAsync(id);
            var lstInvoice = await _context.Invoices.Include(p => p.Account).Include(p => p.OrderStatus).Where(p=> p.AccountId == userLogin.Id).ToListAsync();
            ViewBag.lstBrands = await _context.Brands.Where(p => p.Status == true).ToListAsync();

            foreach (var item in lstInvoice)
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                item.TotalDisplay = item.Total.ToString("#,### ₫", cul.NumberFormat);
            }
            ViewBag.LstInvoice = lstInvoice;

            return View(account);
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,Email,Phone,Address,FullName,IsAdmin,Avatar,Status")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,Email,Phone,Address,FullName,IsAdmin,Avatar,AvatarFile,Status")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

                try
                {
                    account.Status = true;
                    _context.Update(account);
                    await _context.SaveChangesAsync();

                    //  Upload HinhAnh
                    if (account.AvatarFile != null)
                    {
                        if (account.Avatar != "no-image.png")
                        {
                            //  Xóa ảnh hiển thị
                            var fileToDelete = Path.Combine(_webHostEnvironment.WebRootPath, "admin", "assets", "images", "Account", account.Avatar);
                            FileInfo fi = new FileInfo(fileToDelete);
                            fi.Delete();
                        }
                        //Hình ảnh hiển thị
                        var fileName = account.Id.ToString() + Path.GetExtension(account.AvatarFile.FileName);
                        var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "admin", "assets", "images", "Account");
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (FileStream fs = System.IO.File.Create(filePath))
                        {
                            account.AvatarFile.CopyTo(fs);
                            fs.Flush();
                        }
                        account.Avatar = fileName;
                        _context.Update(account);
                        await _context.SaveChangesAsync();
                    }

                    if (account.Avatar == null)
                    {
                        account.Avatar = "no-image.png";
                        _context.Update(account);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["Result"] = MessageConst.UpdateSuccess;
                return RedirectToAction(nameof(Index),account);
            return View(account);
        }

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }

        public async Task<IActionResult> InvoiceDetail(string code)
        {
            Account userLogin = this.GetCurrentUserLogin();
            if (userLogin != null)
            {
                ViewBag.UserLogin = userLogin;
                int totalCart = _context.Carts.Where(p => p.AccountId == userLogin.Id).Count();
                ViewBag.TotalCart = totalCart;
            }
            else
            {
                ViewBag.UserLogin = userLogin;
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var account = await _context.Accounts.FindAsync(userLogin.Id);
            ViewBag.lstBrands = await _context.Brands.Where(p => p.Status == true).ToListAsync();


            var invoiceDetails = await _context.InvoiceDetails.Include(p=> p.Invoice).Where(p => p.Invoice.Code == code).Include(p => p.Product).ToListAsync();
            foreach (var item in invoiceDetails)
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                item.UnitPriceDisplay = item.UnitPrice.ToString("#,### ₫", cul.NumberFormat);
                ViewBag.TotalInvoice = item.Invoice.TotalDisplay = item.Invoice.Total.ToString("#,### ₫", cul.NumberFormat);
                item.IntoMoney = (item.UnitPrice * item.Quantity).ToString("#,### ₫", cul.NumberFormat);
            }
            ViewBag.LstInvoiceDetail = invoiceDetails;

            return View(account);
        }

        public async Task<IActionResult> CencalInvoice(int id)
        {
            Account userLogin = this.GetCurrentUserLogin();
            if (userLogin != null)
            {
                ViewBag.UserLogin = userLogin;
                int totalCart = _context.Carts.Where(p => p.AccountId == userLogin.Id).Count();
                ViewBag.TotalCart = totalCart;
            }
            else
            {
                ViewBag.UserLogin = userLogin;
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var invoice = _context.Invoices.Where(p => p.Id == id).FirstOrDefault();
            ViewBag.lstBrands = await _context.Brands.Where(p => p.Status == true).ToListAsync();
            invoice.OrderStatusId = 6;
            _context.Update(invoice);
            _context.SaveChanges();

            var account = await _context.Accounts.FindAsync(id);
            return RedirectToAction("Index", account);
        }
    }
}
