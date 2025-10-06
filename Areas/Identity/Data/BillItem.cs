using System;
using System.Collections.Generic;

namespace Project.Areas.Identity.Data
{
    public partial class BillItem
    {
        public string Id { get; set; } = null!;
        public string? IdBill { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public int? Number { get; set; }
        public string? UrlImage { get; set; }

        public virtual Bill? IdBillNavigation { get; set; }
    }
}
