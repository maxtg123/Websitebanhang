using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Models;

namespace Shop.Data
{
    public class ShopContext : DbContext
    {
        public ShopContext (DbContextOptions<ShopContext> options)
            : base(options)
        {
        }


        public DbSet<Account> Accounts { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails{ get; set; }
        public DbSet<OrderStatus>  OrderStatuses{ get; set; }
        public DbSet<Product> Products{ get; set; }
        public DbSet<ProductImage> ProductImages{ get; set; }
        public DbSet<ProductType> ProductTypes{ get; set; }
        public DbSet<Slideshow> Slideshows{ get; set; }
        public DbSet<Shop.Models.CPU> CPUs { get; set; }
        public DbSet<Shop.Models.Ram> Rams { get; set; }
        public DbSet<Shop.Models.ProcessHistory> ProcessHistories { get; set; }
        public DbSet<Shop.Models.Warehouse> Warehouses { get; set; }

    }
}
