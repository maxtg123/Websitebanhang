using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class Product
    {
        public int Id { get; set; }
        [NotMapped]
        [Display(Name = "Mã sản phẩm")]
        public string SKU { get; set; }

        [Display(Name = "Tên sản phẩm")]
        [Required(ErrorMessage = "Tên sản phẩm không được bỏ trống")]

        public string Name { get; set; }
        [Display(Name = "Mô tả")]
        [Required(ErrorMessage = "Mô tả không được bỏ trống")]
        public string Description { get; set; }
        [Display(Name = "Số lượng tồn")]
        [Range(0, 999999, ErrorMessage = "Số lượng phải từ 0 trở lên")]
        public int Stock { get; set; }
        [Display(Name = "Hình ảnh")]

        public string Image { get; set; }
        [Display(Name = "Hình ảnh")]

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "RAM")]

        public int RamId { get; set; }

        public Ram Ram { get; set; }

        [Display(Name = "CPU")]
        public int CPUId { get; set; }
        [Display(Name = "Loại RAM CPU")]

        public CPU CPU { get; set; }
        [Display(Name = "Thương hiệu")]

        public int BrandId { get; set; }
        [Display(Name = "Thương hiệu")]

        public Brand Brand { get; set; }
        [Display(Name = "Loại sản phẩm")]

        public int ProductTypeId { get; set; }
        [Required(ErrorMessage = "Giá không được bỏ trống")]
        [Display(Name="Giá")]
        [Range(1, 2100000000, ErrorMessage = "Giá phải lớn hơn 0")]
        public int Price { get; set; }
        [NotMapped]
        [Display(Name = "Giá")]
        public string PriceDisplay { get; set; }

        public ProductType ProductType { get; set; }
        [Display(Name = "Trạng thái")]

        public bool Status { get; set; }

        //[Display(Name = "Thông tin chung")]
        //public string GeneralInfo { get; set; }

        //[Display(Name = "Thông tin khác")]
        //public string OtherInfo { get; set; }

        //public int WarehouseId { get; set; }
        //public Warehouse Warehouse { get; set; }

        public List<Cart> Carts { get; set; }
        //public List<Comment> Comments { get; set; }
        public List<InvoiceDetail> InvoiceDetails { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        [NotMapped]

        [Display(Name = "Hình ảnh chi tiết")]
        public IFormFileCollection GalleryFiles { get; set; }
        [NotMapped]
        [Display(Name = "Giá bán")]
        public int PricePay { get; set; }
        [NotMapped]
        [Display(Name = "Số lượng nhập")]
        public int QuantityImport { get; set; }
        [NotMapped]
        [Display(Name = "Giá nhập")]
        public int PriceImport { get; set; }
        public int View { get; set; }
        public Warehouse Warehouse { get; set; }

    }
}
