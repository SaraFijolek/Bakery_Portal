using WebApplication2.Backery.API.DTO;
using WebApplication2.Properties.Data;

namespace WebApplication2.Backery.Services.Services
{
    public class StatisticsService
    {
        private readonly AppDbContext _context;

        public StatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public StatisticsDto GetStatistics()
        {
            return new StatisticsDto
            {
                TotalUsers = _context.Users.Count(),
                TotalAds = _context.Ads.Count(),
                ActiveAds = _context.Ads.Count(a =>
                  a.Status == "Active"),
                TotalComments = _context.Comments.Count(),
                AverageRating = _context.Ratings.Any()
                    ? _context.Ratings.Average(r => r.Score)
                    : 0,
                NewAdsLast7Days = _context.Ads
                    .Count(a => a.CreatedAt >= DateTime.UtcNow.AddDays(-7))
            };
        }
    }
}
