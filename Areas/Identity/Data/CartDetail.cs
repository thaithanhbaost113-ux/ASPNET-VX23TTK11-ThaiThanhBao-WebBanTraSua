using System;
using System.Collections.Generic;

namespace Project.Areas.Identity.Data
{
    public partial class CartDetail
    {
        public string Id { get; set; } = null!;
        public string Idcart { get; set; } = null!;
        public string IdProduct { get; set; } = null!;
        public int? Number { get; set; }
        public decimal? Price { get; set; }

        public virtual Product IdProductNavigation { get; set; } = null!;
        public virtual Cart IdcartNavigation { get; set; } = null!;
    }
}
