using System;
using System.Collections.Generic;

namespace Project.Areas.Identity.Data
{
    public partial class Cart
    {
        public Cart()
        {
            CartDetails = new HashSet<CartDetail>();
        }

        public string Id { get; set; } = null!;
        public string UserId { get; set; }
        public bool Status { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
    }
}
