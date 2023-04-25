using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Consts;
using Shop.Data;
using Shop.Helper;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    public class LoginController : Controller
    {
        private readonly ShopContext _context;

        public LoginController(ShopContext shopContext)
        {
            _context = shopContext;
        }
        public IActionResult Index()
        {
            if (TempData["ResultUI"] != null)
            {
                ViewBag.ResultUI = TempData["ResultUI"];
            }
            return View();
        }

        public async Task<IActionResult> Login(Account ac)
        {
            var pass = HashPassword.ToMD5(ac.Password);      
            Account auth = await _context.Accounts.FirstOrDefaultAsync(p => p.Status == true && p.Username == ac.Username && p.Password == pass);

            if(auth != null)
            {
                HttpContext.Session.SetInt32("userLoginId", auth.Id);
                if (TempData["ResultUI"] != null)
                {
                    ViewBag.ResultUI = TempData["ResultUI"];
                }

                if (auth.IsAdmin)
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            if (TempData["ResultUI"] != null)
            {
                ViewBag.ResultUI = TempData["ResultUI"];
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> Register(string Username, string Password, string ConfirmPassword)
        {
            if(Password != ConfirmPassword)
            {
                TempData["ResultUI"] = MessageConst.PassWordNotConfirm;
                return RedirectToAction("Index", "Login");
            }

            var checkAC = await _context.Accounts.Where(p => p.Username == Username && p.Status == true).FirstOrDefaultAsync();
            if(checkAC != null)
            {
                return RedirectToAction("Index", "Login");
            }

            Account ac = new Account();            
            ac.Username = Username;
            ac.Password = HashPassword.ToMD5(Password);
            ac.Status = true;
            ac.IsAdmin = false;
            ac.Avatar = "no-image.png";
            _context.Add(ac);
            _context.SaveChanges();
            return RedirectToAction("Index", "Login");
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
