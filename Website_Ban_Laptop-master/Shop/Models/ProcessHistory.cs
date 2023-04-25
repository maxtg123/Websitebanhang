using System;

namespace Shop.Models
{
    public class ProcessHistory
    {
        public int Id { get; set; }
        public int  AccountId { get; set; }
        public Account Account { get; set; }
        public DateTime ProcessDT { get; set; }
        public string Content { get; set; }
        public string HandleWhat { get; set; }
        public int HandleWhatId { get; set; }
    }
}
