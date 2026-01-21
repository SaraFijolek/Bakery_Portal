namespace WebApplication2.Backery.Database.Models
{
    public class SavedSearch
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public int? SubcategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
