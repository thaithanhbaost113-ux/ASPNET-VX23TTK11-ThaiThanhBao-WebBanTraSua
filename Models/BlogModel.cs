namespace Project.Models
{
    public class BlogModel
    {
        public string Id { get; set; } = null!;
        public string? Title { get; set; }
        public string? Conten { get; set; }
        public DateTime? Date { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
