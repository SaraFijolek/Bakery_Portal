namespace WebApplication2.Backery.API.DTO
{
    public class StatisticsDto
    {
        public int TotalUsers { get; set; }
        public int TotalAds { get; set; }
        public int ActiveAds { get; set; }
        public int TotalComments { get; set; }
        public double AverageRating { get; set; }
        public int NewAdsLast7Days { get; set; }

    }
}
