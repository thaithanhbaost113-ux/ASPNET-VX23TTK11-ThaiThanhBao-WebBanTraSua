using System;
using System.Collections.Generic;

namespace Project.Areas.Identity.Data
{
    public partial class Receive
    {
        public Receive()
        {
            RecevieDetails = new HashSet<RecevieDetail>();
        }

        public string Id { get; set; } = null!;
        public string? Provider { get; set; }
        public DateTime? Date { get; set; }
        public virtual Provider? ProviderNavigation { get; set; }
        public virtual ICollection<RecevieDetail> RecevieDetails { get; set; }
    }
}
