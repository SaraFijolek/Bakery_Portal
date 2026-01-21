using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication2.Properties.Models;

namespace WebApplication2.Backery.API.Config
{
    public class RatingConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.HasKey(r => r.RatingId);

            builder
                .HasOne(r => r.FromUser)
                .WithMany(u => u.GivenRatings)
                .HasForeignKey(r => r.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(r => r.ToUser)
                .WithMany(u => u.ReceivedRatings)
                .HasForeignKey(r => r.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
