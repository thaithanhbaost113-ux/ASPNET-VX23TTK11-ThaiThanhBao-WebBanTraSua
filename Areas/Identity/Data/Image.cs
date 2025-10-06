using System;
using System.Collections.Generic;

namespace Project.Areas.Identity.Data
{
    public partial class Image
    {
        public string Id { get; set; } = null!;
        public string? IdProduct { get; set; }
        public string? UrlImage { get; set; }
        public bool? IsMain { get; set; }

        public virtual Product? IdProductNavigation { get; set; }
    }
}
