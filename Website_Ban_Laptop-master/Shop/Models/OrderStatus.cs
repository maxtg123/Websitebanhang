using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class OrderStatus
    {
        public int Id { get; set; }
        public string StatusName { get; set; }
        public List<Invoice> Invoices { get; set; }
    }
}
