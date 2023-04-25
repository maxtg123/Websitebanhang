using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [Display(Name = "Số lượng")]

        public int Quantity { get; set; }
        [Display(Name = "Tạm tính")]
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:#,###.00}")]
        public int Subtotal { get; set; }
        [NotMapped]
        [Display(Name = "Tạm tính")]

        public string SubtotalDisplay { get; set; }

    }
}
