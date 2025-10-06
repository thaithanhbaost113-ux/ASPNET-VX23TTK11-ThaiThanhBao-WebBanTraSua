namespace Project.Models
{
    public class BillModel
    {
        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string Email { get; set; } = null!;
        public DateTime BuyingDate { get; set; }
        public decimal? Total { get; set; }
        public int Pay { get; set; }
        public string Address { get; set; } = null!;
        public bool Status { get; set; }
        public string? Note { get; set; }
        public string PaymentMethods { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
