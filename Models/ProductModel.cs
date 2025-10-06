using Project.Areas.Identity.Data;

namespace Project.Models
{
    public class ProductModel
    {
        public string Id { get; set; } = null!;
        public string? NameProduct { get; set; }
        public string? IdCategory { get; set; }
        public string? Picture { get; set; }
        public string? Category { get; set; }
        public decimal? Price { get; set; }
        public string? Review { get; set; }
        public virtual Image? PictureNavigation { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
        public virtual ICollection<Provider> Providers { get; set; }
    }
}
