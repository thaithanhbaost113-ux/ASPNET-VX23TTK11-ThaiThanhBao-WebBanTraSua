namespace Project.Models
{
    public  class SelctItemModel
    {
        public string Id { get; set; } = null!;
        public string? NameProduct { get; set; } = null!;
        public string? Picture { get; set; }
        //IdItem đại diện cho id của cartitem
        public string? IdItem { get; set; }
        public decimal? Price { get; set; }
        public int? Number { get; set; }
        //public double Title { get; set; }
    }
}
