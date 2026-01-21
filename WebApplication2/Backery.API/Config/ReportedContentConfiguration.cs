using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication2.Properties.Models;

namespace WebApplication2.Backery.API.Config
{
    public class ReportedContentConfiguration : IEntityTypeConfiguration<ReportedContent>
    {
        public void Configure(EntityTypeBuilder<ReportedContent> builder)
        {
            builder.HasKey(r => r.ReportId);

            builder
                .HasOne(r => r.ReporterUser)
                .WithMany(u => u.ReportedContents)
                .HasForeignKey(r => r.ReporterUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(r => r.Admin)
                .WithMany(u => u.ReportedContentAdmins)
                .HasForeignKey(r => r.AdminId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
