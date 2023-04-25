using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        [Display(Name = "Mã hoá đơn")]
        public string Code { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        [Display(Name = "Ngày tạo hoá đơn")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime IssuedDate { get; set; }
        [Display(Name = "Người nhận")]
        [Required(ErrorMessage = "Người nhận không được để trống")]
        public string ShippingName { get; set; }
        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Địa chỉ không được bỏ trống")]
        public string ShippingAddress { get; set; }
        [Display(Name = "SDT người nhận")]
        [Required(ErrorMessage = "SDT không được bỏ trống")]
        public string ShippingPhone { get; set; }

        //public int ShipperId { get; set; }
        //[Required(ErrorMessage = "Tên người giao không được bỏ trống")]
        [Display(Name = "Người giao")]
        public string ShipperName { get; set; }
        [Display(Name = "SDT người giao")]
        //[Required(ErrorMessage = "SDT người giao không được bỏ trống")]
        public string ShipperPhone { get; set; }
        [Display(Name = "Ghi chú")]

        public string Notes { get; set; }
        //public Shipper Shipper { get; set; }
        [Display(Name = "Tổng tiền")]
        public int Total { get; set; }
        [NotMapped]
        [Display(Name = "Tổng tiền")]
        public string TotalDisplay { get; set; }
        [Display(Name = "Trạng thái đơn hàng")]
        public int OrderStatusId { get; set; }
        
        public OrderStatus OrderStatus { get; set; }
        public List<InvoiceDetail> InvoiceDetails { get; set; }

        public bool PaymentMethod { get; set; }
    }
}
