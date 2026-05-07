using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication2.Properties.Models;

namespace WebApplication2.Backery.API.Config
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c => c.CommentId);

            builder
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(c => c.Ad)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.AdId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
    
    }

