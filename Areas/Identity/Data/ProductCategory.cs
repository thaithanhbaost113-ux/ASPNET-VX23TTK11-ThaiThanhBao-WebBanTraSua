namespace Project.Areas.Identity.Data
{
    public class ProductCategory
    {
        public string Id { get; set; }  
        public string Name { get; set; }
        public bool Status { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
