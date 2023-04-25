using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    public class UserModelsController : Controller
    {
        HttpClientHandler _httpClientHandler = new HttpClientHandler();

        UserModel _user = new UserModel();
        List<UserModel> _lstUser = new List<UserModel>();

        public UserModelsController()
        {
            _httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Picture = HttpContext.Session.GetString("picUser");
            HttpContext.Session.SetString("DemoSession", "ValuSession");
            ViewBag.DemoSession = HttpContext.Session.GetString("DemoSession");
            return View();
        }

        private string url = "https://localhost:44388/api/";

        [HttpGet]
        public async Task<List<UserModel>> GetAllUser()
        {
            _lstUser = new List<UserModel>();

            using (var httpClient = new HttpClient(_httpClientHandler))
            {
                using (var response = await httpClient.GetAsync(url + "usermodels/getusermodel"))
                {

                    string apiResponse = await response.Content.ReadAsStringAsync();
                    _lstUser = JsonConvert.DeserializeObject<List<UserModel>>(apiResponse);
                }
            }

            return _lstUser;
        }

        
        public async Task<UserModel> GetById(int id)
        {
            _user = new UserModel();

            using (var httpClient = new HttpClient(_httpClientHandler))
            {
                using (var response = await httpClient.GetAsync(url + "usermodels/getusermodel/" + id))
                {
                    if(response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        _user = JsonConvert.DeserializeObject<UserModel>(apiResponse);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return _user;
        }

        [HttpPost]
        public async Task<string> AddUpdateUser(UserModel user, bool status)
        {
            _user = new UserModel();
            string msg = "Cập nhật thất bại";

            using (var httpClient = new HttpClient(_httpClientHandler))
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(url + "usermodels/PostUserModel/", content))
                {

                    //string apiResponse = await response.Content.ReadAsStringAsync();
                    //_user = JsonConvert.DeserializeObject<UserModel>(apiResponse);
                    if(response.IsSuccessStatusCode)
                    {
                        msg = "Cập nhật thành công";
                    }
                }
            }

            return msg;
        }


        [HttpDelete]
        public async Task<string> Delete(int id)
        {
            string msg = "";

            using (var httpClient = new HttpClient(_httpClientHandler))
            {
                using (var response = await httpClient.DeleteAsync(url + "usermodels/DeleteUserModel/" + id))
                {
                    if(response.IsSuccessStatusCode)
                    {
                        //msg = await response.Content.ReadAsStringAsync();
                        msg = "Xoá thành công";
                    }
                    else
                    {
                        msg = "Xoá thất bại";
                    }
                }
            }

            return msg;
        }

        
    }
}
