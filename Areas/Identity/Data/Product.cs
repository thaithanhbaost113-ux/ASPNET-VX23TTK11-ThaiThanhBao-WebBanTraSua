using System;
using System.Collections.Generic;

namespace Project.Areas.Identity.Data
{
        public partial class Product
        {
            public Product()
            {
                CartDetails = new HashSet<CartDetail>();
                Images = new HashSet<Image>();
                RecevieDetails = new HashSet<RecevieDetail>();
            }

            public string Id { get; set; } = null!;
            public string? NameProduct { get; set; }
            public string? IdCategory { get; set; }
            public string? Category { get; set; }
            public decimal? Price { get; set; }
            public int? Sales { get; set; }
            public int? Status { get; set; }
            public string? Review { get; set; }
            public string? Material { get; set; }
            public int? Number { get; set; }

            public virtual ProductCategory ProductCategory { get; set; }
            public virtual ICollection<CartDetail> CartDetails { get; set; }
            public virtual ICollection<Image> Images { get; set; }
            public virtual ICollection<RecevieDetail> RecevieDetails { get; set; }
        }
}
