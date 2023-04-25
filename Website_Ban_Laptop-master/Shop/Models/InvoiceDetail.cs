using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class InvoiceDetail
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }
        [Display(Name = "Đơn giá")]
        public int UnitPrice { get; set; }
        [NotMapped]
        [Display(Name = "Đơn giá")]
        public string UnitPriceDisplay { get; set; }
        [NotMapped]
        [Display(Name = "Thành tiền")]
        public string IntoMoney { get; set; }
    }
}
