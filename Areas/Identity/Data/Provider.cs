using System;
using System.Collections.Generic;

namespace Project.Areas.Identity.Data
{
    public partial class Provider
    {
        public Provider()
        {
            Receives = new HashSet<Receive>();
        }

        public string Id { get; set; } = null!;
        public string? Name { get; set; }

        public virtual ICollection<Receive> Receives { get; set; }
    }
}
