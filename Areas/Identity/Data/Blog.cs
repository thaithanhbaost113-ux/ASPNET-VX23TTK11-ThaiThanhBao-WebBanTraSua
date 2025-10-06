using System;
using System.Collections.Generic;

namespace Project.Areas.Identity.Data
{
    public partial class Blog
    {
        public string Id { get; set; } = null!;
        public string? Title { get; set; }
        public string? Image { get; set; }
        public string? Conten { get; set; }
        public DateTime? Date { get; set; }
    }
}
