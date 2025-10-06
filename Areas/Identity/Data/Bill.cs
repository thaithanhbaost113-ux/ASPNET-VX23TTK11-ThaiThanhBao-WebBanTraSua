using System;
using System.Collections.Generic;

namespace Project.Areas.Identity.Data
{
    public partial class Bill
    {
        public Bill()
        {
            BillItems = new HashSet<BillItem>();
        }

        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string UserId { get; set; }
        public DateTime BuyingDate { get; set; }
        public decimal Total { get; set; }
        public string Address { get; set; } = null!;
        public int StatusPayment { get; set; }
        public int Status { get; set; }
        public string? Note { get; set; }
        public string Phone { get; set; } = null!;

        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<BillItem> BillItems { get; set; }
    }
}
