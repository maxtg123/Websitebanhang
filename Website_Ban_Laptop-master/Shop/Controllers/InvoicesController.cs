using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Shop.Consts;
using Shop.Data;
using Shop.Models;
using Shop.Orthers;

namespace Shop.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly ShopContext _context;
        public System.Collections.Specialized.NameValueCollection QueryString { get; }
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

        // GET: Invoices/Create
        public IActionResult Create()
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
            ViewBag.TotalCart = _context.Carts.Where(p => p.AccountId == userLogin.Id).Count();

            var lstCart = _context.Carts.Include(p => p.Product).Include(p => p.Account).Where(p=> p.AccountId == userLogin.Id).ToList();
            ViewBag.lstCart = lstCart;
            ViewBag.UserLogin = userLogin;

            var subTotal = 0;
            var transportFee = 0;
            foreach (var item in lstCart)
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
                subTotal += item.Product.Price * item.Quantity;
                item.Product.PriceDisplay = item.Product.Price.ToString("#,###", cul.NumberFormat);
            }
            

            CultureInfo cul2 = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
            ViewBag.SubTotal = subTotal.ToString("#,###", cul2.NumberFormat);

            //var setting = _context.Setting.FirstOrDefault();
            //if (setting != null)
            //{
            //    if (subTotal < setting.TransportFreeMoney)
            //    {
            //        CultureInfo cul1 = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
            //        transportFee += setting.TransportFee;
            //        ViewBag.TransportFree = setting.TransportFee.ToString("#,###", cul1.NumberFormat);
            //    }
            //}
            ViewBag.TotalInsert = transportFee + subTotal;
            ViewBag.Total = (transportFee + subTotal).ToString("#,###", cul2.NumberFormat);

            if (TempData["ResultUI"] != null)
            {
                ViewBag.ResultUI = TempData["ResultUI"];
            }
            return View();
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int payment_method,[Bind("Id,Code,AccountId,IssuedDate,ShippingName,ShippingAddress,ShippingPhone,ShipperId,Notes,Total,OrderStatusId")] Invoice invoice)
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

            if (string.IsNullOrEmpty(invoice.ShippingName))
            {
                TempData["ResultUI"] = MessageConst.PleaseEnterShippingName;
                return RedirectToAction("Create");
            }

            if (string.IsNullOrEmpty(invoice.ShippingAddress))
            {
                TempData["ResultUI"] = MessageConst.PleaseEnterShippingAddress;
                return RedirectToAction("Create");
            }

            if (string.IsNullOrEmpty(invoice.ShippingPhone))
            {
                TempData["ResultUI"] = MessageConst.PleaseEnterShippingPhone;
                return RedirectToAction("Create");
            }

            if (payment_method == 0)
            {
                bool isWhile = true;
                //Thêm hoá đơn
                do
                {
                    Guid guild1 = Guid.NewGuid();
                    invoice.Code = guild1.ToString("N").Substring(0, 5);
                    var invoiceCheck = _context.Invoices.FirstOrDefault(p => p.Code == invoice.Code);
                    if (invoiceCheck == null)
                    {
                        isWhile = false;
                    }
                    else
                    {
                        isWhile = true;
                    }
                } while (isWhile);

                DateTime dt = DateTime.Now;
                invoice.AccountId = userLogin.Id;
                invoice.IssuedDate = dt;
                invoice.OrderStatusId = OrderStatusConst.WaitConfirm;
                invoice.PaymentMethod = payment_method == 0?false:true;
                _context.Invoices.Add(invoice);
                _context.SaveChanges();

                //Thêm chi tiết hoá đơn
                var lstCart = _context.Carts.Include(p => p.Product).Where(p=> p.AccountId == userLogin.Id).ToList();
                foreach (var item in lstCart)
                {
                    InvoiceDetail invoiceDetail = new InvoiceDetail();
                    invoiceDetail.InvoiceId = invoice.Id;
                    invoiceDetail.ProductId = item.ProductId;
                    invoiceDetail.Quantity = item.Quantity;
                    invoiceDetail.UnitPrice = item.Product.Price;
                    _context.Add(invoiceDetail);
                }
                _context.SaveChanges();

                //Trừ số lượng tồn
                foreach (var item in lstCart)
                {
                    var pro = _context.Products.Where(p => p.Id == item.ProductId).FirstOrDefault();
                    pro.Stock -= item.Quantity;
                    _context.Update(item);
                    _context.Remove(item);
                }
                _context.SaveChanges();
                TempData["ResultUI"] = MessageConst.PaySuccess;
                return RedirectToAction("Index", "Carts");
            }
            else
            {
                bool isWhile = true;
                //Thêm hoá đơn
                do
                {
                    Guid guild1 = Guid.NewGuid();
                    invoice.Code = guild1.ToString("N").Substring(0, 5);
                    var invoiceCheck = _context.Invoices.FirstOrDefault(p => p.Code == invoice.Code);
                    if (invoiceCheck == null)
                    {
                        isWhile = false;
                    }
                    else
                    {
                        isWhile = true;
                    }
                } while (isWhile);

                DateTime dt = DateTime.Now;
                invoice.AccountId = userLogin.Id;
                invoice.IssuedDate = dt;
                invoice.OrderStatusId = OrderStatusConst.WaitConfirm;
                invoice.PaymentMethod = payment_method ==0?false:true;
                _context.Invoices.Add(invoice);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("InvoiceId", invoice.Id);

                ////Thêm chi tiết hoá đơn
                //var lstCart = _context.Carts.Include(p => p.Product).Where(p => p.AccountId == userLogin.Id).ToList();
                //foreach (var item in lstCart)
                //{
                //    InvoiceDetail invoiceDetail = new InvoiceDetail();
                //    invoiceDetail.InvoiceId = invoice.Id;
                //    invoiceDetail.ProductId = item.ProductId;
                //    invoiceDetail.Quantity = item.Quantity;
                //    invoiceDetail.UnitPrice = item.Product.Price;
                //    _context.Add(invoiceDetail);
                //}
                //_context.SaveChanges();

                ////Trừ số lượng tồn
                //foreach (var item in lstCart)
                //{
                //    var pro = _context.Products.Where(p => p.Id == item.ProductId).FirstOrDefault();
                //    pro.Stock -= item.Quantity;
                //    _context.Update(item);
                //    _context.Remove(item);
                //}
                //_context.SaveChanges();
                return RedirectToAction("Payment",invoice);

            }

            return View(invoice);
        }

        public ActionResult Payment(Invoice invoice)
        {
            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOSWUZ20220718";
            string accessKey = "SbPex1VxsN663Us7";
            string serectkey = "1sEPSrbZsA23ovgE8inUCpNEPrzM9SXw";
            string orderInfo = "Thanh toán đơn hàng";
            string returnUrl = "https://localhost:44358/Invoices/ConfirmPaymentClient";
            string notifyurl = "https://6de0-118-69-72-241.ap.ngrok.io/Invoices/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test

            string amount = invoice.Total.ToString();
            string orderid = invoice.Code;
            string requestId = invoice.Code;
            //string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        //Khi thanh toán xong ở cổng thanh toán Momo, Momo sẽ trả về một số thông tin, trong đó có errorCode để check thông tin thanh toán
        //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
        //Tham khảo bảng mã lỗi tại: https://developers.momo.vn/#/docs/aio/?id=b%e1%ba%a3ng-m%c3%a3-l%e1%bb%97i
        public ActionResult ConfirmPaymentClient()
        {
            var request = Request.QueryString.ToString();
            //hiển thị thông báo cho người dùng
            var result = request.Split('&');
            var b = result[11];
            var check = b.Split('=');

            if (check[1] == "0")
            {
                TempData["ResultUI"] = MessageConst.PaymentSuccess;

                Account userLogin = this.GetCurrentUserLogin();
                var a = Request;
                int invId = (int)HttpContext.Session.GetInt32("InvoiceId");
                //Thêm chi tiết hoá đơn
                var lstCart = _context.Carts.Include(p => p.Product).Where(p => p.AccountId == userLogin.Id).ToList();
                foreach (var item in lstCart)
                {
                    InvoiceDetail invoiceDetail = new InvoiceDetail();
                    invoiceDetail.InvoiceId = invId;
                    invoiceDetail.ProductId = item.ProductId;
                    invoiceDetail.Quantity = item.Quantity;
                    invoiceDetail.UnitPrice = item.Product.Price;
                    _context.Add(invoiceDetail);
                }
                _context.SaveChanges();

                //Trừ số lượng tồn
                foreach (var item in lstCart)
                {
                    var pro = _context.Products.Where(p => p.Id == item.ProductId).FirstOrDefault();
                    pro.Stock -= item.Quantity;
                    _context.Update(item);
                    _context.Remove(item);
                }
                _context.SaveChanges();

                return RedirectToAction("Index", "Carts");
            }
            else
            {
                TempData["ResultUI"] = MessageConst.PaymentFail;
                return RedirectToAction("Index", "Carts");
            }
            return View();
        }

        [HttpPost]
        public void SavePayment()
        {
            //Account userLogin = this.GetCurrentUserLogin();
            //var a = Request;
            //int invId = (int)HttpContext.Session.GetInt32("InvoiceId");
            ////Thêm chi tiết hoá đơn
            //var lstCart = _context.Carts.Include(p => p.Product).Where(p => p.AccountId == userLogin.Id).ToList();
            //foreach (var item in lstCart)
            //{
            //    InvoiceDetail invoiceDetail = new InvoiceDetail();
            //    invoiceDetail.InvoiceId = invId;
            //    invoiceDetail.ProductId = item.ProductId;
            //    invoiceDetail.Quantity = item.Quantity;
            //    invoiceDetail.UnitPrice = item.Product.Price;
            //    _context.Add(invoiceDetail);
            //}
            //_context.SaveChanges();

            ////Trừ số lượng tồn
            //foreach (var item in lstCart)
            //{
            //    var pro = _context.Products.Where(p => p.Id == item.ProductId).FirstOrDefault();
            //    pro.Stock -= item.Quantity;
            //    _context.Update(item);
            //    _context.Remove(item);
            //}
            //_context.SaveChanges();

            //cập nhật dữ liệu vào db
        }
    }
}
