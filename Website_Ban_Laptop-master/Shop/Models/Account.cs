using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được bỏ trống")]
        [Display(Name ="Tên đăng nhập")]
        [MinLength(5, ErrorMessage = "Tên đăng nhập tối thiểu 5 ký tự")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        [Display(Name = "Mật khẩu")]
        [MinLength(8, ErrorMessage = "Mật khẩu tối thiểu 8 ký tự")]
        public string Password { get; set; }

        [NotMapped]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "SDT")]
        public string Phone { get; set; }


        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Display(Name = "Tài khoản Admin")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string Avatar { get; set; }

        [NotMapped]
        [Display(Name = "Ảnh đại diện")]
        public IFormFile AvatarFile { get; set; }

        [Display(Name = "Trạng thái ")]
        public bool Status { get; set; }
        public List<Cart> Carts { get; set; }
        //public List<Comment> Comments { get; set; }
        public List<Invoice> Invoices { get; set; }
        public List<ProcessHistory> ProcessHistories { get; set; }

    }
}
