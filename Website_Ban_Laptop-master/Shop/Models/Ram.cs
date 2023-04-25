using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class Ram
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số lượng RAM")]
        [Range(1, 500, ErrorMessage = "Dung lượng RAM phải lớn hơn 0 và nhỏ hơn 500")]
        [Display(Name = "Dung lượng RAM")]
        public int Amount { get; set; }
        [Display(Name = "Trạng thái")]
        public bool Status { get; set; }
        [NotMapped]
        public int ProductCount { get; set; }

        public List<Product> Products { get; set; }

    }
}
