using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shop.Consts;
using Shop.Data;
using Shop.Models;
using PayPal.Api;

namespace Shop.Controllers
{
    public class CartsController : Controller
    {
        private readonly ShopContext _context;
        private readonly string _clientId;
        private readonly string _secretKey;
        public double TyGiaUSD = 23300;

        public CartsController(ShopContext context, IConfiguration configuration)
        {
            _context = context;
            _clientId = configuration["PaypalSettings:ClientId"];
            _secretKey = configuration["PaypalSettings:SecretKey"];
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

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            Account userLogin = this.GetCurrentUserLogin();
            if (userLogin != null)
            {
                ViewBag.UserLogin = userLogin;
            }
            else
            {
                ViewBag.UserLogin = userLogin;
                return RedirectToAction("Index", "Login", new { area = "" });
            }
            if (TempData["ResultUI"] != null)
            {
                ViewBag.ResultUI = TempData["ResultUI"];
            }

            ViewBag.TotalCart = _context.Carts.Where(p => p.AccountId == userLogin.Id).Count();
            ViewBag.lstBrands = await _context.Brands.Where(p => p.Status == true).ToListAsync();

            var total = 0;

            var shopContext = _context.Carts.Include(c => c.Account).Include(c => c.Product);
            foreach (var item in shopContext)
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                string priceFormat = item.Product.Price.ToString("#,###", cul.NumberFormat);
                item.Product.PriceDisplay = priceFormat;

                var subtotal = item.Subtotal = item.Product.Price * item.Quantity;
                string subtotalFormat = item.Subtotal.ToString("#,###", cul.NumberFormat);
                item.SubtotalDisplay = subtotalFormat;

                total += subtotal;
            }

            CultureInfo cul2 = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
            ViewBag.SubTotal = total.ToString("#,###", cul2.NumberFormat);

            //var setting = await _context.Setting.FirstOrDefaultAsync();
            //if(setting != null)
            //{
            //    if(total < setting.TransportFreeMoney)
            //    {
            //        CultureInfo cul1 = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
            //        total += setting.TransportFee;
            //        ViewBag.TransportFree = setting.TransportFee.ToString("#,###", cul1.NumberFormat);
            //    }
            //}

            ViewBag.Total = total.ToString("#,###", cul2.NumberFormat);
            return View(await shopContext.ToListAsync());
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Account)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int productId, int quantity, int cartId, int? detailId)
        {
            Account userLogin = this.GetCurrentUserLogin();
            if (userLogin != null)
            {
                ViewBag.UserLogin = userLogin;
            }
            else
            {
                ViewBag.UserLogin = userLogin;
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            //Nếu số lượng = 0
            if (quantity <= 0)
            {
                TempData["ResultUI"] = MessageConst.PleaseEnterQuantity;
                var pro = await _context.Products.Where(p => p.Id == productId).FirstOrDefaultAsync();
                return RedirectToAction("Product_Detail", "Products", pro);
            }

            //Kiểm tra số lượng tồn
            if (!CheckProductInStock(productId, quantity, cartId))
            {
                TempData["ResultUI"] = MessageConst.NotEnoughQuantity;
                var pro = await _context.Products.Where(p => p.Id == productId).FirstOrDefaultAsync();
                return RedirectToAction("Product_Detail", "Products", pro);
            }

            Cart checkCart = _context.Carts.FirstOrDefault(p => p.ProductId == productId && p.AccountId == userLogin.Id);
            if (checkCart != null)
            {
                checkCart.Quantity += quantity;
                await _context.SaveChangesAsync();

                TempData["ResultUI"] = MessageConst.UpdateCartSuccess;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                Cart newCart = new Cart();
                newCart.AccountId = userLogin.Id;
                newCart.ProductId = productId;
                newCart.Quantity = quantity;
                _context.Add(newCart);
                await _context.SaveChangesAsync();

                TempData["ResultUI"] = MessageConst.AddCartSuccess;
                return RedirectToAction(nameof(Index));
            }
            
           
            return View();
        }

        public bool CheckProductInStock(int productId, int quantity, int cartId = 0)
        {
            Account userLogin = this.GetCurrentUserLogin();

            if (cartId != 0)
            {
                Cart cart = _context.Carts.FirstOrDefault(p => p.ProductId == userLogin.Id && p.ProductId == productId && p.Quantity + quantity > p.Product.Stock);
                Product pro = _context.Products.FirstOrDefault(p => p.Stock < quantity && p.Id == productId);

                if (cart == null)
                {
                    if (pro == null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                Product pro = _context.Products.FirstOrDefault(p => p.Stock < quantity && p.Id == productId);

                if (pro == null)
                {
                    return true;
                }
            }

            return false;
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id", cart.AccountId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", cart.ProductId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountId,ProductId,Quantity")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id", cart.AccountId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var cart = await _context.Carts.FindAsync(id);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            TempData["ResultUI"] = MessageConst.UpdateCartSuccess;
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<string> UpdateCart(string id)
        {
            int idProduct = Convert.ToInt32(id.Split('-')[0]);
            int quantity = Convert.ToInt32(id.Split('-')[1]);
            int idCart = Convert.ToInt32(id.Split('-')[2]);
            //Nếu số lượng = 0
            if (quantity <= 0)
            {
                //Xoá sản phẩm
                TempData["ResultUI"] = MessageConst.UpdateCartSuccess;
                var cartRemove = await _context.Carts.FindAsync(idCart);
                _context.Remove(cartRemove);
                await _context.SaveChangesAsync();
                return "Cập nhật thất bại";
            }

            //Kiểm tra số lượng tồn
            if (!CheckProductInStock(idProduct, quantity, idCart))
            {
                TempData["ResultUI"] = MessageConst.NotEnoughQuantity;
                return "Cập nhật thất bại";
            }

            var cart = await _context.Carts.FindAsync(idCart);
            cart.Quantity = quantity;
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
            TempData["ResultUI"] = MessageConst.UpdateCartSuccess;
            return  "Cập nhật thành Công";
        }

        public async Task<IActionResult> DeleteAllCart()
        {
            Account userLogin = this.GetCurrentUserLogin();
            if(userLogin != null)
            {
                var lstCart = await _context.Carts.Where(p => p.AccountId == userLogin.Id).ToListAsync();
                foreach (var item in lstCart)
                {
                    _context.Carts.Remove(item);
                }
                await _context.SaveChangesAsync();
                TempData["ResultUI"] = MessageConst.UpdateCartSuccess;
            }
            return RedirectToAction(nameof(Index));
        }


        public IActionResult PaypalCheckout()
        {
            //var environment = new SandboxEnvironment(_clientId, _secretKey);
            //var client = new PayPalHttpClient(environment);

            return View();
        }
    }
}
