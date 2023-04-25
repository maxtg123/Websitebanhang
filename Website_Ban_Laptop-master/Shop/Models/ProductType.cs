﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class ProductType
    {
        public int Id { get; set; }
        [Display(Name = "Loại sản phẩm")]
        [Required(ErrorMessage = "Tên loại sản phẩm không được bỏ trống")]
        public string Name { get; set; }
        [Display(Name = "Trạng thái")]

        public bool Status { get; set; }
        [NotMapped]
        public int ProductCount { get; set; }

        public List<Product> Products { get; set; }
    }
}
