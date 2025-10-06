using System;
using System.Collections.Generic;

namespace Project.Areas.Identity.Data
{
    public partial class RecevieDetail
    {
        public string Id { get; set; } = null!;
        public string? IdReceive { get; set; }
        public string? IdProduct { get; set; }
        public int? Number { get; set; }
        public decimal? Price { get; set; }

        public virtual Product? IdProductNavigation { get; set; }
        public virtual Receive? IdReceiveNavigation { get; set; }
    }
}
