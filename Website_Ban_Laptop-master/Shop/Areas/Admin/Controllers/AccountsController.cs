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
    public class AccountsController : Controller
    {
        private readonly ShopContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountsController(ShopContext context, IWebHostEnvironment iWebHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = iWebHostEnvironment;
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
        
        // GET: Admin/Accounts
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
            
            if (TempData["Result"] != null)
            {
                ViewBag.ShowMessage = TempData["Result"];
            }

            return View(await _context.Accounts.Where(p=> p.Status == true).ToListAsync());
        }

        // GET: Admin/Accounts/Create
        public IActionResult Create()
        {

            #region Kiểm tra User Login: Phúc 13/07/22
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

            if (TempData["Result"] != null)
            {
                ViewBag.ShowMessage = TempData["Result"];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,Email,Phone,Address,FullName,IsAdmin,Avatar,AvatarFile,Status")] Account account)
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

            var checkAC = await _context.Accounts.Where(p => p.Username == account.Username && p.Status == true).FirstOrDefaultAsync();
            if (checkAC != null)
            {
                TempData["Result"] = MessageConst.UserNameIsExists;
                return RedirectToAction("Create", "Accounts");
            }

                #region Hash Password: Phúc 12/07/22
                account.Password = HashPassword.ToMD5(account.Password);
            #endregion

            account.IsAdmin = true;
            account.Status = true;
            _context.Add(account);
            await _context.SaveChangesAsync();

            //  Upload HinhAnh
            if (account.AvatarFile != null)
            {
                    
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
            TempData["Result"] = MessageConst.AddSuccess;
            return RedirectToAction("Index", "Accounts");
            
            return View(account);
        }

        // GET: Admin/Accounts/Delete/5
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

            var account = await _context.Accounts.FirstOrDefaultAsync(p => p.Id == id);

            if(account.IsAdmin)
            {
                var countAccount = await _context.Accounts.Where(p => p.IsAdmin == true && p.Status == true).CountAsync();
                if(countAccount < 2)
                {
                    TempData["Result"] = MessageConst.ThisIsTheLastAccountAdmin;
                    return RedirectToAction("Index", "Accounts");
                }
            }

            account.Status = false;
            _context.Update(account);
            await _context.SaveChangesAsync();

            TempData["Result"] = MessageConst.DeleteSuccess;
            return RedirectToAction("Index", "Accounts");
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
