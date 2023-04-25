using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class Warehouse
    {
        public int Id { get; set; }

        [Display(Name = "Tên sản phẩm")]
        [NotMapped]
        public string Name { get; set; }

        [Display(Name = "Số lượng nhập")]
        [Range(1, 999999, ErrorMessage = "Số lượng phải lớn hơn 0")]

        public int Quantity { get; set; }
        [Range(1, 2100000000, ErrorMessage = "Giá phải lớn hơn 0")]

        [Display(Name = "Giá nhập")]
        public int Price { get; set; }

        [Display(Name = "Ngày nhập")]
        public DateTime IssuedDate { get; set; }

        [NotMapped]
        [Display(Name = "Giá nhập")]
        public string PriceDisplay { get; set; }

        [NotMapped]
        [Display(Name = "Thành tiền")]
        public string TotalDisplay { get; set; }

        [NotMapped]
        [Display(Name = "Số lượng nhập")]
        public int QuantityImport { get; set; }
        [NotMapped]
        [Display(Name = "Giá nhập")]
        public int PriceImport { get; set; }

        [NotMapped]
        [Display(Name = "Thành tiền")]
        public int Total { get; set; }
        [NotMapped]
        [Display(Name = "Thành tiền")]
        public string ProductName { get; set; }

        [NotMapped]
        [Display(Name = "Hình ảnh")]
        public string ProductImage { get; set; }


        public int ProductId { get; set; }

        public  List<Product> Products { get; set; }

    }
}
