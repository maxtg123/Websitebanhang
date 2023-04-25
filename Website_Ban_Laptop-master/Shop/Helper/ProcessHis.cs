using Shop.Consts;
using Shop.Data;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Helper
{
    public class ProcessHis
    {
        private readonly ShopContext _context;

        public ProcessHis(ShopContext shopContext)
        {
            _context = shopContext;
        }
        public async void ProCessHisIns(ProcessHistory processHistory)
        {
            processHistory.ProcessDT = DateTime.Now;
            _context.Add(processHistory);
            await _context.SaveChangesAsync();

        }
    }
}
